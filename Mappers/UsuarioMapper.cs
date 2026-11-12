using ApiClinica.Models;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;

namespace ApiClinica.Mappers;

public class UsuarioMapper : IUsuarioMapper
{
    public Usuario ToEntity(UsuarioCreateDTO dto)
    {
        return new Usuario
        {
            Login = dto.Login,
            Perfil = dto.Perfil,
            Senha = dto.Senha
        };
    }

    public UsuarioReadDTO ToReadDTO(Usuario usuario)
    {
        return new UsuarioReadDTO
        {
            Id = usuario.Id,
            Login = usuario.Login,
            Perfil = usuario.Perfil,
            Senha = usuario.Senha
        };
    }
    public void ApplyUpdate(UsuarioUpdateDTO dto, Usuario entidade)
    {
        if (dto.Login is not null) entidade.Login = dto.Login;
        if (dto.Senha is not null) entidade.Senha = dto.Senha;
        if (dto.Perfil is not null) entidade.Perfil = dto.Perfil;
    }
}