using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiClinica.Controllers;

[ApiController]
// Define a rota base da controller
// Exemplo: api/auth/login
[Route("api/auth")]
public class AuthController : ControllerBase
{
    // Contexto do banco de dados
    private readonly AppDbContext _context;

    // Acesso às configurações do appsettings.json
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // Endpoint para cadastrar usuário
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        // Verifica se já existe usuário com esse login
        var usuarioExistente = await _context.Usuarios.AnyAsync(u => u.Login == dto.Login);

        if (usuarioExistente)
        {
            return BadRequest(new { mensagem = "Usuário já existe" });
        }

        // Cria novo usuário
        var usuario = new Usuario
        {
            Login = dto.Login,
            Senha = dto.Senha,
            Perfil = dto.Perfil ?? "usuario",
        };

        // Adiciona usuário no banco
        _context.Usuarios.Add(usuario);

        // Salva alterações
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Usuário criado com sucesso" });
    }

    // Endpoint de login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        // Busca usuário no banco
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
            u.Login == dto.Login && u.Senha == dto.Senha
        );

        // Retorna erro se login inválido
        if (usuario == null)
            return Unauthorized(new { mensagem = "Usuário ou senha inválidos" });

        // Informações que serão gravadas dentro do token
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, usuario.Login),
            new Claim(ClaimTypes.Role, usuario.Perfil),
        };

        // Chave usada para assinar o token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        // Define algoritmo de criptografia
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Cria o token JWT
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            // Tempo de expiração do token
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        // Converte token para string
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Retorna token para o usuário
        return Ok(new { token = tokenString });
    }
}
