using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.Mappers;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly AppDbContext _context;
    public PacientesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/paciente
    [HttpGet]
    public async Task<IActionResult> GetPacientes()
    {
        var pacientes = await _context.Pacientes.ToListAsync();

        var pacientesDTO = pacientes
            .Select(p => PacienteMapper.ToDTO(p))
            .ToList();

        return Ok(pacientesDTO);
    }

    // GET: api/paciente/id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPacienteById(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente == null)
            return NotFound();

        return Ok(PacienteMapper.ToDTO(paciente));
    }

    // POST: api/paciente
    [HttpPost]
    public async Task<IActionResult> CreatePaciente([FromBody] PacienteCreateDTO dto)
    {
        if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
        {
            return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });
        }

        var paciente = PacienteMapper.ToModel(dto);

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        var pacienteDTO = PacienteMapper.ToDTO(paciente);
        
        return CreatedAtAction(nameof(GetPacienteById), new { id = paciente.Id }, pacienteDTO);
    }

    // PUT: api/paciente
    [HttpPut]
    public async Task<IActionResult> UpdatePaciente(int Id, [FromBody] Paciente paciente)
    {
        if (Id != paciente.Id)
        {
            return BadRequest("O ID da URL não confere com o ID do corpo da requisição.");
        }

        var existente = await _context.Pacientes.FindAsync(Id); //busca
        if (existente == null)
            return NotFound();
        
        if (paciente.DataNasc > DateOnly.FromDateTime(DateTime.Today))
        {
            return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });
        }

        existente.Nome = paciente.Nome; //atualiza os campos
        existente.Email = paciente.Email;
        existente.Telefone = paciente.Telefone;
        existente.DataNasc = paciente.DataNasc;
        existente.Cpf = paciente.Cpf;
        
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/paciente/id
    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeletePaciente(int Id)
    {
        var paciente = await _context.Pacientes.FindAsync(Id);
        if (paciente == null)
            return NotFound();

        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}