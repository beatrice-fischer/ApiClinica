using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ConsultasController : ControllerBase
{

    private const int DuracaoConsultaMinutos = 30;

    private readonly AppDbContext _context;
    public ConsultasController(AppDbContext context)
    {
        _context = context;
    }
    // GET: api/consulta
    [HttpGet]
    public async Task<IActionResult> GetConsultas()
    {
        var consultas = await _context.Consultas.ToListAsync();
        return Ok(consultas);
    }

    // GET: api/consulta/id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetConsultaById(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        if (consulta == null)
            return NotFound();

        return Ok(consulta);
    }

    // POST: api/consulta
    [HttpPost]
    public async Task<IActionResult> CreateConsulta([FromBody] Consulta consulta)
    {
        var erro = await ValidarConsulta(consulta.PacienteId, consulta.MedicoId, consulta.DataHora);

        if (erro != null)
            return erro;

        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetConsultaById), new { id = consulta.Id }, consulta);
    }

    // PUT: api/consulta
    [HttpPut]
    public async Task<IActionResult> UpdateConsulta(int Id, [FromBody] Consulta consulta)
    {
        if (Id != consulta.Id)
        {
            return BadRequest("O ID da URL não confere com o ID do corpo da requisição.");
        }

        var existente = await _context.Consultas.FindAsync(Id); //busca
        if (existente == null)
            return NotFound();

        var erro = await ValidarConsulta(consulta.PacienteId, consulta.MedicoId, consulta.DataHora, Id);

        if (erro != null)
            return erro;

        existente.PacienteId = consulta.PacienteId; //atualiza os campos
        existente.MedicoId = consulta.MedicoId;
        existente.DataHora = consulta.DataHora;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/medico/id
    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteConsulta(int Id)
    {
        var consulta = await _context.Consultas.FindAsync(Id); //busca
        if (consulta == null)
            return NotFound();

        _context.Consultas.Remove(consulta);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    //================================================================
    //  VALIDAÇÃO das regras DE NEGÓCIO para agendamento de consultas
    //================================================================
    private async Task<IActionResult?> ValidarConsulta(
        int pacienteId,
        int medicoId,
        DateTime dataHora,
        int? idIgnorar = null)
    {
        //VALIDAÇÃO do ID de paciente
        var paciente = await _context.Pacientes.FindAsync(pacienteId);
        if (paciente is null)
            return BadRequest(new { mensagem = $"Paciente {pacienteId} não encontrado." });

        //VALIDAÇÃO do ID de médico
        var medico = await _context.Medicos.FindAsync(medicoId);
        if (medico is null)
            return BadRequest(new { mensagem = $"Médico {medicoId} não encontrado." });

        //VALIDAÇÃO da data
        if (dataHora < DateTime.Now)
            return BadRequest(new { mensagem = "Não é possível agendar uma consulta em uma data/horário no passado." });

        //VALIDAÇÃO de conflito de horário (janela de 30 minutos antes e depois da consulta)
        var inicioJanela = dataHora.AddMinutes(-DuracaoConsultaMinutos);
        var fimJanela    = dataHora.AddMinutes(DuracaoConsultaMinutos);

        //VALIDAÇÃO de conflito na agenda do médico
        bool conflitoMedico = await _context.Consultas.AnyAsync(c =>
            (idIgnorar == null || c.Id != idIgnorar) &&
            c.MedicoId == medicoId &&
            //Não utilizar >= e <= para evitar conflito de horário com consultas que iniciam ou terminam no mesmo horário
            c.DataHora > inicioJanela &&
            c.DataHora < fimJanela);

        if (conflitoMedico)
            return Conflict(new { mensagem = $"Este médico possui consulta em horário conflitante (intervalo mínimo de {DuracaoConsultaMinutos} minutos)." });

        //VALIDAÇÃO de conflito na agenda do paciente
        bool conflitoPaciente = await _context.Consultas.AnyAsync(c =>
            (idIgnorar == null || c.Id != idIgnorar) &&
            c.PacienteId == pacienteId &&
            //Não utilizar >= e <= para evitar conflito de horário com consultas que iniciam ou terminam no mesmo horário
            c.DataHora > inicioJanela &&
            c.DataHora < fimJanela);

        if (conflitoPaciente)
            return Conflict(new { mensagem = $"Este paciente possui consulta em horário conflitante (intervalo mínimo de {DuracaoConsultaMinutos} minutos)." });

        return null;
    }
}