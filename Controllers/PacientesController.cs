using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.Mappers;
using ApiClinica.Validators;

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
            .Select(p => PacienteMapper.ToReadDTO(p))
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

        return Ok(PacienteMapper.ToReadDTO(paciente));
    }

    /* TESTE MANUAL
       GET http://localhost:5052/api/pacientes
       Esperado: 200 com array. Nenhum item traz o campo Cpf.
    */

    // POST: api/paciente
    [HttpPost]
    public async Task<IActionResult> CreatePaciente([FromBody] PacienteCreateDTO dto)
    {
        var erro = ContatoValidator.ValidarEmail(dto.Email) ?? ContatoValidator.ValidarTelefone(dto.Telefone);

        if (erro != null)
            return BadRequest(new { mensagem = erro });

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

    // PATCH: api/pacientes/id
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchPaciente(int id, [FromBody] PacienteUpdateDTO dto)
    {
        var existente = await _context.Pacientes.FindAsync(id);
        if (existente is null)
            return NotFound();

        string? erro = null;

        if (dto.Email is not null)
            erro = ContatoValidator.ValidarEmail(dto.Email);

        if (erro is null && dto.Telefone is not null)
            erro = ContatoValidator.ValidarTelefone(dto.Telefone);

        if (erro is not null)
            return BadRequest(new { mensagem = erro });

        if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
            return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });

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
            return Conflict(new { mensagem = "Não é possível excluir o paciente, existem consultas futuras agendadas." });
        }

        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

/* ============================================================================
 * TESTES MANUAIS — PacientesController
 * Base: http://localhost:5052   (confira a porta na linha "Now listening on:")
 * Todo POST e PATCH precisa do header  Content-Type: application/json
 * ----------------------------------------------------------------------------
 *
 * [1] GET /api/pacientes
 *     200 com array. Nenhum item traz o campo Cpf — o ReadDTO nao o expoe.
 *
 * [2] GET /api/pacientes/{id}
 *     200 para id existente (pegue um da listagem acima).
 *     404 para id inexistente.
 *
 * [3] POST /api/pacientes
 *     {
 *       "Nome": "Ana Souza",
 *       "Email": "ana.souza@example.com",
 *       "Telefone": "(47) 98888-8888",
 *       "DataNasc": "1995-03-24",
 *       "Cpf": "529.982.247-25"
 *     }
 *     201 + header Location. A resposta NAO traz o Cpf.
 *
 *     Variacoes de erro (troque apenas o campo indicado):
 *       "Cpf": "111.111.111-11"       -> 400  digitos verificadores invalidos
 *       "Cpf": "529.982.247-25"       -> 409  duplicado, na segunda vez
 *       "Email": "nao-e-email"        -> 400
 *       "Telefone": "123"             -> 400
 *       "DataNasc": "2090-01-01"      -> 400
 *
 *     Outros CPFs validos para testar: 111.444.777-35 e 123.456.789-09
 *
 * [4] PATCH /api/pacientes/{id}
 *     { "Nome": "Ana Souza Lima" }
 *     200. Faca um GET em seguida: Email, Telefone e DataNasc INTACTOS.
 *     Se algum voltar nulo, o ApplyUpdate esta sobrescrevendo campo nao enviado.
 *
 *       { "Cpf": "111.444.777-35" }   -> 200 e o Cpf NAO muda (campo nao editavel)
 *       { "Email": "nao-e-email" }    -> 400
 *       { "Telefone": "123" }         -> 400
 *       { "DataNasc": "2090-01-01" }  -> 400
 *       PATCH em /api/pacientes/9999  -> 404
 *
 * [5] DELETE /api/pacientes/{id}
 *     409 se o paciente tiver consulta futura agendada.
 *     204 depois que a consulta for excluida.
 *     404 para id inexistente.
 *
 * ----------------------------------------------------------------------------
 * COMO LER UM 400: o corpo da resposta diz de onde ele veio.
 *   {"type":"...rfc9110...","errors":{...}}  -> validacao automatica do DTO
 *                                               (atributos como [EmailAddress])
 *   {"mensagem":"..."}                       -> validacao escrita no controller
 *
 * A validacao para no PRIMEIRO erro encontrado. Mandando dois campos invalidos,
 * so o primeiro aparece na resposta.
 * ========================================================================== */