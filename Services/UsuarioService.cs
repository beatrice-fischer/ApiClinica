using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace ApiClinica.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _context;
    private readonly IUsuarioMapper _mapper;
    private readonly IConfiguration _configuration;

    public UsuarioService(AppDbContext context, IUsuarioMapper mapper, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<Resultado<List<UsuarioReadDTO>>> ListarAsync()
    {
        var Usuarios = await _context.Usuarios.ToListAsync();

        var dtos = Usuarios.Select(p => _mapper.ToReadDTO(p)).ToList();

        return Resultado<List<UsuarioReadDTO>>.Ok(dtos);
    }

    public async Task<Resultado<UsuarioReadDTO>> BuscarPorIdAsync(int id)
    {
        var Usuario = await _context.Usuarios.FindAsync(id);

        if (Usuario is null)
            return Resultado<UsuarioReadDTO>.NaoEncontrado();

        return Resultado<UsuarioReadDTO>.Ok(_mapper.ToReadDTO(Usuario));
    }

    public async Task<Resultado<UsuarioReadDTO>> CriarAsync(UsuarioCreateDTO dto)
    {
        var usuarioExistente = await _context.Usuarios.AnyAsync(u => u.Login == dto.Login);

        if (usuarioExistente)
        {
            return Resultado<UsuarioReadDTO>.Conflito("Usuário já existe");
        }

        // Cria novo usuário
        var usuario = _mapper.ToEntity(dto);

        // Adiciona usuário no banco
        _context.Usuarios.Add(usuario);

        // Salva alterações
        await _context.SaveChangesAsync();

        return Resultado<UsuarioReadDTO>.Ok(_mapper.ToReadDTO(usuario)); ;
    }

    public async Task<Resultado<UsuarioReadDTO>> AtualizarAsync(int id, UsuarioUpdateDTO dto)
    {
        var existente = await _context.Usuarios.FindAsync(id);

        if (existente is null)
            return Resultado<UsuarioReadDTO>.NaoEncontrado();

        _mapper.ApplyUpdate(dto, existente);

        await _context.SaveChangesAsync();

        return Resultado<UsuarioReadDTO>.Ok(_mapper.ToReadDTO(existente));
    }

    public async Task<Resultado> ExcluirAsync(int id)
    {
        var Usuario = await _context.Usuarios.FindAsync(id);

        if (Usuario is null)
            return Resultado.NaoEncontrado();

        _context.Usuarios.Remove(Usuario);
        await _context.SaveChangesAsync();

        return Resultado.Ok();
    }

    public async Task<string> AuthAsync(LoginDTO dto)
    {
        // Busca usuário no banco
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
            u.Login == dto.Login && u.Senha == dto.Senha
        );

        // Retorna erro se login inválido
        if (usuario == null)
            return null;

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
        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Retorna token para o usuário
        return (tokenString);
    }
}
