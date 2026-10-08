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
    [HttpDelete("{Id}")]
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

/* ============================================================================
 * TESTES MANUAIS — MedicosController
 * Base: http://localhost:5052   (confira a porta na linha "Now listening on:")
 * Todo POST e PATCH precisa do header  Content-Type: application/json
 * ----------------------------------------------------------------------------
 *
 * [1] GET /api/medicos
 *     200 com array.
 *
 * [2] GET /api/medicos/{id}
 *     200 para id existente, 404 para inexistente.
 *
 * [3] POST /api/medicos
 *     {
 *       "Nome": "Dr. Carlos Silva",
 *       "Email": "carlos.silva@example.com",
 *       "Telefone": "(47) 97777-7777",
 *       "CRM": "CRM-SC 12345"
 *     }
 *     201 + header Location.
 *
 *     Variacoes de erro:
 *       "Email": "arroba-faltando"    -> 400
 *       "Telefone": "abc"             -> 400
 *
 * [4] PATCH /api/medicos/{id}
 *     { "Telefone": "(47) 96666-6666" }
 *     204. Faca um GET em seguida: Nome, Email e CRM INTACTOS.
 *
 *       { "Telefone": "xyz" }         -> 400
 *       { "Email": "sem-arroba" }     -> 400
 *       PATCH em /api/medicos/9999    -> 404
 *
 * [5] DELETE /api/medicos/{id}
 *     409 se o medico tiver consulta futura agendada.
 *     204 depois que a consulta for excluida.
 *     404 para id inexistente.
 *
 * ----------------------------------------------------------------------------
 * Formato de telefone aceito pelo ContatoValidator: DDD de 2 digitos e numero
 * de 8 ou 9 digitos, com ou sem parenteses, espaco e hifen.
 *   (47) 98888-8888   (47)988888888   47988888888   47 3333-4444
 * ========================================================================== */
