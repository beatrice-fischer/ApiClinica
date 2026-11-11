using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Validators;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Services;

public class MedicoService : IMedicoService
{
    private readonly AppDbContext _context;
    private readonly IMedicoMapper _mapper;

    public MedicoService(AppDbContext context, IMedicoMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Resultado<List<MedicoReadDTO>>> ListarAsync()
    {
        var medicos = await _context.Medicos.ToListAsync();

        var dtos = medicos
            .Select(m => _mapper.ToReadDTO(m))
            .ToList();

        return Resultado<List<MedicoReadDTO>>.Ok(dtos);
    }

    public async Task<Resultado<MedicoReadDTO>> BuscarPorIdAsync(int id)
    {
        var medico = await _context.Medicos.FindAsync(id);

        if (medico is null)
            return Resultado<MedicoReadDTO>.NaoEncontrado();

        return Resultado<MedicoReadDTO>.Ok(_mapper.ToReadDTO(medico));
    }

    public async Task<Resultado<MedicoReadDTO>> CriarAsync(MedicoCreateDTO dto)
    {
        var erro = ContatoValidator.ValidarEmail(dto.Email)
            ?? ContatoValidator.ValidarTelefone(dto.Telefone);

        if (erro is not null)
            return Resultado<MedicoReadDTO>.Invalido(erro);

        var medico = _mapper.ToEntity(dto);

        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();

        return Resultado<MedicoReadDTO>.Ok(_mapper.ToReadDTO(medico));
    }

    public async Task<Resultado> AtualizarAsync(int id, MedicoUpdateDTO dto)
    {
        var existente = await _context.Medicos.FindAsync(id);

        if (existente is null)
            return Resultado.NaoEncontrado();

        string? erro = null;

        if (dto.Email is not null)
            erro = ContatoValidator.ValidarEmail(dto.Email);

        if (erro is null && dto.Telefone is not null)
            erro = ContatoValidator.ValidarTelefone(dto.Telefone);

        if (erro is not null)
            return Resultado.Invalido(erro);

        _mapper.ApplyUpdate(dto, existente);

        await _context.SaveChangesAsync();

        return Resultado.Ok();
    }

    public async Task<Resultado> ExcluirAsync(int id)
    {
        var medico = await _context.Medicos.FindAsync(id);

        if (medico is null)
            return Resultado.NaoEncontrado();

        bool temConsultaFutura = await _context.Consultas.AnyAsync(c =>
            c.MedicoId == id && c.DataHora > DateTime.Now);

        if (temConsultaFutura)
            return Resultado.Conflito("Não é possível excluir o médico, pois ele possui consultas futuras agendadas.");

        _context.Medicos.Remove(medico);
        await _context.SaveChangesAsync();

        return Resultado.Ok();
    }
}