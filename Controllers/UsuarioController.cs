using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Models;
using ApiClinica.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiClinica.Controllers;

[ApiController]
// Define a rota base da controller
// Exemplo: api/auth/login

[Route("api/auth")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuarioController(IUsuarioService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsuarios()
    {
        var r = await _service.ListarAsync();
        return r.Sucesso ? Ok(r.Valor) : Traduzir(r.Tipo, r.Erro);
    }

    // Endpoint para cadastrar usuário
    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register(UsuarioCreateDTO dto)
    {
        var r = await _service.CriarAsync(dto);

        if (!r.Sucesso)
            return Traduzir(r.Tipo, r.Erro);
        else
            return Ok("Usuário criado com sucesso!");
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UsuarioUpdateDTO dto)
    {
        var r = await _service.AtualizarAsync(id, dto);
        return r.Sucesso ? Ok(r.Valor) : Traduzir(r.Tipo, r.Erro);
    }

    // Endpoint de login
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<Resultado> Login(LoginDTO dto)
    {
        var usuarios = await _service.AuthAsync(dto);
        if(usuarios == null)
        {
            return Resultado.NaoAutorizado("Usuário ou senha inválidos");
        }

        return Resultado.LoginToken(usuarios);
    }


    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
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
