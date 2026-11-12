using ApiClinica.DTOs;
using ApiClinica.Services;

namespace ApiClinica.Interfaces
{
    public interface IUsuarioService
    {
        Task<Resultado<List<UsuarioReadDTO>>> ListarAsync();
        Task<Resultado<UsuarioReadDTO>> BuscarPorIdAsync(int id);
        Task<Resultado<UsuarioReadDTO>> CriarAsync(UsuarioCreateDTO dto);
        Task<Resultado<UsuarioReadDTO>> AtualizarAsync(int id, UsuarioUpdateDTO dto);
        Task<Resultado> ExcluirAsync(int id);
        Task<string> AuthAsync(LoginDTO dto);
    }
}
