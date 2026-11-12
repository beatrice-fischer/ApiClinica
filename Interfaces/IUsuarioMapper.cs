using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Interfaces
{
    public interface IUsuarioMapper
    {
        void ApplyUpdate(UsuarioUpdateDTO dto, Usuario existente);
        Usuario ToEntity(UsuarioCreateDTO dto);
        UsuarioReadDTO ToReadDTO(Usuario usuario);
    }
}
