using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.interfaces;
using ApiClinica.Validators;
using Microsoft.EntityFrameworkCore;
    
namespace ApiClinica.Services;

public class PacienteService : IPacienteService
{
    private readonly AppDbContext _context;
    private readonly IPacienteMaper _mapper;

    public PacienteService(AppDbContext context, IPacienteMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Resultado<List<PacienteReadDTO>>> ListarAsync()
    {
        var pacientes = await _context.Pacientes.ToListAsync();

        var dtos = pacientes
            .Select(pacientes => _mapper.ToReadDTO(p))
            .ToList();

        return Resultado<ListarAsync<PacienteReadDTO>>.Ok(dtos);
    }

    public async Task<Resultado<PacienteReadDTO>> BuscarPorIdAsync (int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente is null)
            return Resultado<PacienteReadDTO>.NaoEncontrado();

        return Resultado<PacienteReadDTO>.Ok(_mapper.ToReadDTO(paciente));
    }

    public async Task<Resultado<PacienteReadDTO>>
    CriarAsync(PacienteCreateDTO dto)
    {
        var erro = ContatoValidator.ValidarEmail(dto.Email)
            ?? Contato.Validator.ValidarTelefone(dto.Telefone);

        if (erro is not null)
            return Resultado<PacienteReadDTO>.Invalido(erro);

        if (dto.DataNasc > Dateonly.FromDateTime(DateTime.Today))
            return Resultado<PacienteReadDTO>.Invalido("Data de nascimento não pode ser futura.");
        
        if (!CpfValidator.EhValido(dto.Cpf))
            return Resultado<PacienteReadDTO>.Invalido("CPF inválido.");

        if (await _context.Pacientes.AnyAsync( p => p.Cpf == dto.Cpf))
            return Resultado<PacienteReadDTO>.Conflito("Já existe um paciente com este CPF.");

        var paciente = _mapper.ToEntity(dto);

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        return Resultado<PacienteReadDTO>.Ok(_mapper.ToReadDTO(paciente));
    }

    public async Task<Resultado<PacienteReadDTO>> AtualizarAsync (int id, PacienteReadDTO dto)
    {
        var existente = await _context.Pacientes.FindAsync(id);

        if (existente is null)
            return Resultado<PacienteReadDTO>.NaoEncontrado();

        string? erro = null;

        if (dto.Email is not null)
            erro = ContatoValidator.ValidarEmail(dto.Email);    
        
        if (erro is null && dto.Telefone is not null)
            erro = ContatoValidator.ValidarTelefone(dto.Telefone);

        if (erro is not null)
            return Resultado<PacienteReadDTO>.Invalido(erro);
        
        if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
            return Resultado<PacienteReadDTO>.Invalido("Data de Nascimento não pode ser futura.");
        
        _mapper.ApplyUpdate(dto, existente);
        
        await _context.SaveChangesAsync();

        return Resultado<PacienteReadDTO>.Ok(_mapper.ToReadDTO(existente));
    }

    public async Task<Resultado> ExcluirAsync(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (PacienteService is null)
            return Resultado.NaoEncontrado();
        
        bool temConsultaFutura = await _context.Consultas.AnyAsync (c => c.PacienteId == id && c.DataHora > DateTime.Now);

        if (temConsultaFutura)
            return Resultado.Conflito("Não é possivel excluir o paciente, existem consultas futuras agendadas.");

        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();

        return Resultado.Ok();
    }
}