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
    //método que busca os pacientes no banco
    public async Task<IActionResult> GetPacientes()
    {
        var pacientes = await _context.Pacientes.ToListAsync();

        var pacientesDTO = pacientes
            .Select(p => PacienteMapper.ToReadDTO(p))
            .ToList();

        return Ok(pacientesDTO);
    }

    // GET: api/paciente/id
    [HttpGet("{id}")]
    //método que busca o paciente no banco por ID
    public async Task<IActionResult> GetPacienteById(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente == null)
            return NotFound();

        return Ok(PacienteMapper.ToReadDTO(paciente));
    }

    // POST: api/paciente
    [HttpPost]
    public async Task<IActionResult> CreatePaciente([FromBody] PacienteCreateDTO dto)
    {
        var erro = ValidarEmail(dto.Email) ?? ValidarTelefone(dto.Telefone);

        if (erro!=null)
        return BadRequest(new {message = erro});

        if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
        {
            return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });
        }

        if (!CpfValidator.EhValido(dto.Cpf))
        return BadRequest(new { mensagem = "CPF inválido." });

        if (await _context.Pacientes.AnyAsync(p => p.Cpf == dto.Cpf))
        return Conflict(new { mensagem = "Já existe um paciente com este CPF." });

        var paciente = PacienteMapper.ToEntity(dto);

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        var pacienteDTO = PacienteMapper.ToReadDTO(paciente);
        
        return CreatedAtAction(nameof(GetPacienteById), new { id = paciente.Id }, pacienteDTO);
    }

    // PATCH: api/paciente
    [HttpPatch("{Id}")]
    public async Task<IActionResult> PatchPaciente(int Id, [FromBody] PacienteUpdateDTO dto)
    {

        ValidadorEmailTelefoneController.ValidarEmail(dto.Email);
        
        ValidadorEmailTelefoneController.ValidarTelefone(dto.Telefone);

        if (!CpfValidator.EhValido(dto.Cpf))
        return BadRequest(new { mensagem = "CPF inválido." });

        var existente = await _context.Pacientes.FindAsync(Id); //busca
        if (existente == null)
            return NotFound();
        
        if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
        {
            return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });
        }


        PacienteMapper.ApplyUpdate(dto, existente);
        
        await _context.SaveChangesAsync();

        return Ok(PacienteMapper.ToReadDTO(existente));
    }

    // DELETE: api/paciente/id
    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeletePaciente(int Id)
    {
        var paciente = await _context.Pacientes.FindAsync(Id);
        if (paciente == null)
            return NotFound();

        bool temConsultaFutura = await _context.Consultas.AnyAsync(c =>
        c.PacienteId == Id && c.DataHora > DateTime.Now);

        if (temConsultaFutura)
        {
            return Conflict(new { message = "Não é possível excluir o paciente, existem consultas futuras agendadas." });
        }

        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}