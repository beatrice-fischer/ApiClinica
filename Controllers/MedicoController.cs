using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicoController : ControllerBase
{
    private readonly AppDbContext _context;
    public MedicoController(AppDbContext context)
    {
        _context = context;
    }
    private static List<Medico> medicos = new();
    private static int _nextId = 1;

    // GET: api/medico
    [HttpGet]
    public async Task<IActionResult> GetMedicos()
    {
        var medicos = await _context.Medicos.ToListAsync();
        return Ok(medicos);
    }

    // GET: api/medico/id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMedicoById(int id)
    {
        var medico = await _context.Medicos.FindAsync(id);

        if (medico == null)
            return NotFound();

        return Ok(medico);
    }

    // POST: api/medico
    [HttpPost]
    public async Task<IActionResult> CreateMedico([FromBody] Medico medico)
    {
        medico.Id = _nextId++;

        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMedicoById), new { id = medico.Id }, medico);
    }

    // PUT: api/medico
    [HttpPut]
    public async Task<IActionResult> UpdateMedico(int Id, [FromBody] Medico medico)
    {
        if (Id != medico.Id)
        {
            return BadRequest("O ID da URL não confere com o ID do corpo da requisição.");
        }

        var existente = await _context.Medicos.FindAsync(Id); //busca
        if (existente == null)
            return NotFound();

        existente.Nome = medico.Nome; //atualiza os campos
        existente.Email = medico.Email;
        existente.Telefone = medico.Telefone;
        existente.CRM = medico.CRM;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/medico/id
    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteMedico(int Id)
    {
        var medico = await _context.Medicos.FindAsync(Id); //busca
        if (medico == null)
            return NotFound();

        _context.Medicos.Remove(medico);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}