using ApiClinica.Data;
using ApiClinica.Mappers;
using Microsoft.AspNetCore.Mvc;
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

        var consultasDTO = consultas            // Converte cada entidade em DTO de leitura
            .Select(c => ConsultaMapper.ToReadDTO(c))
            .ToList();

        return Ok(consultasDTO);
    }

    // GET: api/consulta/id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetConsultaById(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        if (consulta == null)
            return NotFound();

        return Ok(ConsultaMapper.ToReadDTO(consulta));
    }

    // POST: api/consulta
    [HttpPost]
    public async Task<IActionResult> CreateConsulta([FromBody] ConsultaCreateDTO dto)
    {
        var erro = await ValidarConsulta(dto.PacienteId, dto.MedicoId, dto.DataHora);

        if (erro != null)
            return erro;

        var consulta = ConsultaMapper.ToEntity(dto);   // DTO -> entidade

        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();             // aqui o banco preenche o Id

        return CreatedAtAction(nameof(GetConsultaById), new { id = consulta.Id },
                               ConsultaMapper.ToReadDTO(consulta));
    }

    // PATCH: api/consulta
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateConsulta(int id, [FromBody] ConsultaUpdateDTO dto)
    {
        var existente = await _context.Consultas.FindAsync(id);
        if (existente == null)
            return NotFound();

        // Valores finais: o que veio no corpo, ou o que ja esta gravado
        var pacienteId = dto.PacienteId ?? existente.PacienteId;
        var medicoId = dto.MedicoId ?? existente.MedicoId;
        var dataHora = dto.DataHora ?? existente.DataHora;

        var erro = await ValidarConsulta(pacienteId, medicoId, dataHora, id);

        if (erro != null)
            return erro;

        ConsultaMapper.ApplyUpdate(dto, existente);    // atualiza apenas os campos nao nulos

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
        var fimJanela = dataHora.AddMinutes(DuracaoConsultaMinutos);

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

/* ============================================================================
 * TESTES MANUAIS — ConsultasController
 * Base: http://localhost:5052   (confira a porta na linha "Now listening on:")
 * Todo POST e PATCH precisa do header  Content-Type: application/json
 *
 * ATENCAO: crie um paciente e um medico primeiro, e use os ids reais deles.
 * Mande a data SEM o "Z" no fim — com "Z" o .NET interpreta como UTC e a
 * comparacao contra DateTime.Now erra em 3 horas.
 * ----------------------------------------------------------------------------
 *
 * [1] GET /api/consultas
 *     200 com array.
 *
 * [2] GET /api/consultas/{id}
 *     200 para id existente, 404 para inexistente.
 *
 * [3] POST /api/consultas
 *     {
 *       "PacienteId": 1,
 *       "MedicoId": 1,
 *       "DataHora": "2026-10-21T10:00:00"
 *     }
 *     201 + header Location.
 *
 *     Variacoes de erro:
 *       "PacienteId": 9999                  -> 400  paciente nao existe
 *       "MedicoId": 9999                    -> 400  medico nao existe
 *       "DataHora": "2020-01-01T10:00:00"   -> 400  no passado
 *
 * [4] JANELA DE 30 MINUTOS
 *     Depois de criar a consulta das 10:00 acima, para o MESMO medico:
 *       "DataHora": "2026-10-21T10:15:00"   -> 409  sobrepoe
 *       "DataHora": "2026-10-21T10:29:00"   -> 409  sobrepoe
 *       "DataHora": "2026-10-21T10:30:00"   -> 201  encosta, nao sobrepoe
 *       "DataHora": "2026-10-21T09:30:00"   -> 201  encosta pelo outro lado
 *     A mesma regra vale trocando o medico e repetindo o paciente.
 *     Se 10:30 devolver 409, a comparacao esta com >= e <= em vez de > e <.
 *
 * [5] PATCH /api/consultas/{id}
 *     { "DataHora": "2026-10-21T14:00:00" }
 *     204. Faca um GET em seguida: PacienteId e MedicoId continuam maiores
 *     que zero. Se virarem 0, o ApplyUpdate esta atribuindo sem checar HasValue.
 *
 *       { "MedicoId": 2 }
 *         Revalida o medico NOVO contra o horario ANTIGO. Se o medico 2 ja
 *         tiver consulta naquele horario, 409.
 *
 *       Reagendar para o MESMO horario que a consulta ja ocupa -> 204, nao 409.
 *       Se der 409, o idIgnorar nao esta sendo passado e a consulta entra em
 *       conflito consigo mesma.
 *
 *       PATCH em /api/consultas/9999        -> 404
 *
 * [6] DELETE /api/consultas/{id}
 *     204. Id inexistente -> 404.
 *
 * ========================================================================== */