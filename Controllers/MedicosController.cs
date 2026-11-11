using Microsoft.AspNetCore.Mvc;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Services;
using Microsoft.AspNetCore.Authorization;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MedicosController : ControllerBase
{
    private readonly IMedicoService _service;
    public MedicosController(IMedicoService service)
    {
        _service = service;
    }

    // GET: api/medicos
    [HttpGet]
    public async Task<IActionResult> GetMedicos()
    {
        var r = await _service.ListarAsync();
        return r.Sucesso ? Ok(r.Valor) : Traduzir(r.Tipo, r.Erro);
    }

    // GET: api/medicos/id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMedicoById(int id)
    {
        var r = await _service.BuscarPorIdAsync(id);
        return r.Sucesso ? Ok(r.Valor) : Traduzir(r.Tipo, r.Erro);
    }

    // POST: api/medicos
    [HttpPost]
    public async Task<IActionResult> CreateMedico([FromBody] MedicoCreateDTO dto)
    {
        var r = await _service.CriarAsync(dto);

        if (!r.Sucesso)
            return Traduzir(r.Tipo, r.Erro);

        return CreatedAtAction(nameof(GetMedicoById), new { id = r.Valor!.Id }, r.Valor);
    }

    // PATCH: api/medicos/id
    [HttpPatch("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateMedico(int id, [FromBody] MedicoUpdateDTO dto)
    {
        var r = await _service.AtualizarAsync(id, dto);
        return r.Sucesso ? NoContent() : Traduzir(r.Tipo, r.Erro);
    }

    // DELETE: api/medicos/id
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteMedico(int id)
    {
        var r = await _service.ExcluirAsync(id);
        return r.Sucesso ? NoContent() : Traduzir(r.Tipo, r.Erro);
    }

    private IActionResult Traduzir(TipoErro tipo, string? erro) => tipo switch
    {
        TipoErro.NaoEncontrado => NotFound(),
        TipoErro.Invalido => BadRequest(new { mensagem = erro }),
        TipoErro.Conflito => Conflict(new { mensagem = erro }),
        _ => StatusCode(500)
    };
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
