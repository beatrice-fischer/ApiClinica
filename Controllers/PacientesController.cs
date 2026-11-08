using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _service;

    public PacientesController(IPacienteService service)
    {
        _service = service;
    }

    // GET: api/paciente
    [HttpGet]
    public async Task<IActionResult> GetPacientes()
    {
        var r = await _service.ListarAsync();
        return r.Sucesso ? Ok(r.Valor) : Traduzir(r.Tipo, r.Erro);
    }

    // GET: api/paciente/id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPacienteById(int id)
    {
        var r = await _service.BuscarPorIdAsync(id);
        return r.Sucesso ? Ok(r.Valor) : Traduzir(r.Tipo, r.Erro);
    }

    /* TESTE MANUAL
       GET http://localhost:5052/api/pacientes
       Esperado: 200 com array. Nenhum item traz o campo Cpf.
    */

    // POST: api/paciente
    [HttpPost]
    public async Task<IActionResult> CreatePaciente([FromBody] PacienteCreateDTO dto)
    {
        var r = await _service.CriarAsync(dto);

        if (!r.Sucesso)
            return Traduzir(r.Tipo, r.Erro);

        return CreatedAtAction(nameof(GetPacienteById), new { id = r.Valor!.Id }, r.Valor);
    }

    // PATCH: api/pacientes/id
    [HttpPatch("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PatchPaciente(int id, [FromBody] PacienteUpdateDTO dto)
    {
        var r = await _service.AtualizarAsync(id, dto);
        return r.Sucesso ? Ok(r.Valor) : Traduzir(r.Tipo, r.Erro);
    }

    // DELETE: api/paciente/id
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeletePaciente(int id)
    {
        var r = await _service.ExcluirAsync(id);
        return r.Sucesso ? NoContent() : Traduzir(r.Tipo, r.Erro);
    }

    // Único lugar do projeto que decide código de status
    private IActionResult Traduzir(TipoErro tipo, string? erro) =>
        tipo switch
        {
            TipoErro.NaoEncontrado => NotFound(),
            TipoErro.Invalido => BadRequest(new { mensagem = erro }),
            TipoErro.Conflito => Conflict(new { mensagem = erro }),
            _ => StatusCode(500),
        };
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
