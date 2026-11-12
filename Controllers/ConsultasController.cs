using Microsoft.AspNetCore.Mvc;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ApiClinica.Services;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class ConsultasController : ControllerBase
{

    private readonly IConsultaService _service;
    public ConsultasController(IConsultaService service)
    {
        _service = service;
    }
    // GET: api/consulta
    [HttpGet]
    public async Task<IActionResult> GetConsultas()
    {
        var r = await _service.ListarAsync();
        return r.Sucesso ? Ok(r.Valor) : Traduzir(r.Tipo, r.Erro);
    }

    // GET: api/consulta/id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetConsultaById(int id)
    {
        var r = await _service.BuscarPorIdAsync(id);
        return r.Sucesso ? Ok(r.Valor) : Traduzir(r.Tipo, r.Erro);
    }

    // POST: api/consulta
    [HttpPost]
    public async Task<IActionResult> CreateConsulta([FromBody] ConsultaCreateDTO dto)
    {
        var r = await _service.CriarAsync(dto);

        if (!r.Sucesso)
            return Traduzir(r.Tipo, r.Erro);

        return CreatedAtAction(nameof(GetConsultaById), new { id = r.Valor!.Id }, r.Valor);
    }

    // PATCH: api/consulta
    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateConsulta(int id, [FromBody] ConsultaUpdateDTO dto)
    {
        var r = await _service.AtualizarAsync(id, dto);
        return r.Sucesso ? NoContent() : Traduzir(r.Tipo, r.Erro);
    }


    // DELETE: api/medico/id
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConsulta(int id)
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