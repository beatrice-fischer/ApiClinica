using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultasController : ControllerBase
{
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
        var erro = await ValidarConsulta(consulta);

        if (erro != null)
            return BadRequest(new { mensagem = erro });

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

        var erro = await ValidarConsulta(consulta, Id);

        if (erro != null)
            return BadRequest(new {mensagem = erro});

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

    //VALIDAÇÃO das regras
    private async Task<string?> ValidarConsulta(Consulta consulta, int? idIgnorar = null)
    {
        if (consulta.DataHora < DateTime.Now)
        {
            return "Não é possível agendar uma consulta em uma data/horário no passado.";
        }

        bool conflitoMedico = await _context.Consultas.AnyAsync(c =>
            c.Id != idIgnorar &&
            c.MedicoId == consulta.MedicoId &&
            c.DataHora == consulta.DataHora);

        if (conflitoMedico)
        {
            return "Este médico já possui uma consulta marcada para este mesmo horário.";
        }

        bool conflitoPaciente = await _context.Consultas.AnyAsync(c =>
            c.Id != idIgnorar &&
            c.PacienteId == consulta.PacienteId &&
            c.DataHora == consulta.DataHora);

        if (conflitoPaciente)
        {
            return "Este paciente já possui uma consulta marcada para este mesmo horário.";
        }

        await _context.Pacientes.FindAsync(consulta.PacienteId);
        

        return null;
    }
}