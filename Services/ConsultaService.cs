using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Services;

public class ConsultaService : IConsultaService
{
    private const int DuracaoConsultaMinutos = 30;

    private readonly AppDbContext _context;
    private readonly IConsultaMapper _mapper;

    public ConsultaService(AppDbContext context, IConsultaMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Resultado<List<ConsultaReadDTO>>> ListarAsync()
    {
        var consultas = await _context.Consultas.ToListAsync();

        var dtos = consultas
            .Select(c => _mapper.ToReadDTO(c))
            .ToList();
        
        return Resultado<List<ConsultaReadDTO>>.Ok(dtos);
    }
    
    public async Task<Resultado<ConsultaReadDTO>> BuscarPorIdAsync(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        if (consulta is null)
            return Resultado<ConsultaReadDTO>.NaoEncontrado();

        return Resultado<ConsultaReadDTO>.Ok(_mapper.ToReadDTO(consulta));
    }

    public async Task<Resultado<ConsultaReadDTO>> CriarAsync(ConsultaCreateDTO dto)
    {
        var erro = await ValidarConsulta(dto.PacienteId, dto.MedicoId, dto.DataHora);

        if (erro is not null)
            return erro.Tipo == TipoErro.Conflito
                ? Resultado<ConsultaReadDTO>.Conflito(erro.Erro!)
                : Resultado<ConsultaReadDTO>.Invalido(erro.Erro!);

        var consulta = _mapper.ToEntity(dto);
        
        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();
        
        return Resultado<ConsultaReadDTO>.Ok(_mapper.ToReadDTO(consulta));
    }

    public async Task<Resultado> AtualizarAsync(int id, ConsultaUpdateDTO dto)
    {
        var existente = await _context.Consultas.FindAsync(id);

        if (existente is null)
            return Resultado.NaoEncontrado();

        //Valores finais
        var pacienteId = dto.PacienteId ?? existente.PacienteId;
        var medicoId = dto.MedicoId ?? existente.MedicoId;
        var dataHora = dto.DataHora ?? existente.DataHora;

        var erro = await ValidarConsulta(pacienteId, medicoId, dataHora, id);

        if (erro is not null)
            return erro;
        
        _mapper.ApplyUpdate(dto, existente);

        await _context.SaveChangesAsync();

        return Resultado.Ok();
    }

    public async Task<Resultado> ExcluirAsync(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        if (consulta is null)
            return Resultado.NaoEncontrado();

        _context.Consultas.Remove(consulta);
        await _context.SaveChangesAsync();

        return Resultado.Ok();
    }

    //=====VALIDAÇÕES=====

    private async Task<Resultado?> ValidarConsulta(
        int pacienteId,
        int medicoId,
        DateTime dataHora,
        int? idIgnorar = null)
    {
        var paciente = await _context.Pacientes.FindAsync(pacienteId);
        if (paciente is null)
            return Resultado.Invalido($"Paciente {pacienteId} não encontrado.");
        
        var medico = await _context.Medicos.FindAsync(medicoId);
        if (medico is null)
            return Resultado.Invalido($"Médico {medicoId} não encontrado.");
            
        if (dataHora < DateTime.Now)
            return Resultado.Invalido("Não é possível agendar uma consulta em uma data/horário no passado.");

        var inicioJanela = dataHora.AddMinutes(-DuracaoConsultaMinutos);
        var fimJanela = dataHora.AddMinutes(DuracaoConsultaMinutos);

        bool conflitoMedico = await _context.Consultas.AnyAsync(c =>
            (idIgnorar == null || c.Id != idIgnorar) &&
            c.MedicoId == medicoId &&
            c.DataHora > inicioJanela &&
            c.DataHora < fimJanela);
        
        if (conflitoMedico)
            return Resultado.Conflito($"Este médico possui consulta em horário conflitante (intervalo mínimo de {DuracaoConsultaMinutos} minutos).");

        bool conflitoPaciente = await _context.Consultas.AnyAsync(c =>
            (idIgnorar == null || c.Id != idIgnorar) &&
            c.PacienteId == pacienteId &&
            c.DataHora > inicioJanela &&
            c.DataHora < fimJanela);
        
        if (conflitoPaciente)
            return Resultado.Conflito($"Este paciente possui consulta em horário conflitante (intervalo mínimo de {DuracaoConsultaMinutos} minutos).");

        return null;
    }
}