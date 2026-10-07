using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiClinica.Data;
using ApiClinica.Mappers;
using ApiClinica.Validators;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly AppDbContext _context;
    public MedicosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/medico
    [HttpGet]
    public async Task<IActionResult> GetMedicos()
    {
        var medicos = await _context.Medicos.ToListAsync(); // Busca todos os médicos no banco

        var medicosDTO = medicos // Converte cada entidade em DTO de leitura
            .Select(m => MedicoMapper.ToReadDTO(m))
            .ToList();

        return Ok(medicosDTO);
    }

    // GET: api/medico/id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMedicoById(int id)
    {
        var medico = await _context.Medicos.FindAsync(id); // Busca o médico pelo Id

        if (medico == null) // 404 se não existir
            return NotFound();

        return Ok(MedicoMapper.ToReadDTO(medico)); // Devolve o DTO de leitura
    }

    // POST: api/medico
    [HttpPost]
    public async Task<IActionResult> CreateMedico([FromBody] MedicoCreateDTO dto)
    {
        var erro = ContatoValidator.ValidarEmail(dto.Email) ?? ContatoValidator.ValidarTelefone(dto.Telefone);

        if (erro != null)
            return BadRequest(new { mensagem = erro });

        var medico = MedicoMapper.ToEntity(dto); // Converte o DTO em entidade

        _context.Medicos.Add(medico); // Salva no banco
        await _context.SaveChangesAsync();

        // 201 com o DTO de leitura e o link do GET por Id
        return CreatedAtAction(nameof(GetMedicoById), new { id = medico.Id }, MedicoMapper.ToReadDTO(medico));
    }

    // PATCH: api/medicos/id
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateMedico(int id, [FromBody] MedicoUpdateDTO dto)
    {
        var existente = await _context.Medicos.FindAsync(id); //Busca o médico
        if (existente == null)
            return NotFound();

        string? erro = null; //Valida campos que foram alterados

        if (dto.Email is not null)
            erro = ContatoValidator.ValidarEmail(dto.Email);

        if (erro == null && dto.Telefone is not null)
            erro = ContatoValidator.ValidarTelefone(dto.Telefone);

        if (erro != null)   
            return BadRequest(new { mensagem = erro });

        MedicoMapper.ApplyUpdate(dto, existente); // atualiza apenas os campos não nulos

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/medico/id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMedico(int id)
    {
        var medico = await _context.Medicos.FindAsync(id); // Busca o médico
        if (medico == null)
            return NotFound();

        // Verifica se há consultas futuras desse médico
        bool temConsultaFutura = await _context.Consultas.AnyAsync(c =>
            c.MedicoId == id && c.DataHora > DateTime.Now);

        if (temConsultaFutura) // Bloqueia a exclusão com 409 Conflict
        {
            return Conflict(new { mensagem = "Não é possível excluir o médico, pois ele possui consultas futuras agendadas." });
        }

        _context.Medicos.Remove(medico); // Remove do banco
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
