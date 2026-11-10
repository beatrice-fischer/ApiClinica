using ApiClinica.DTOs;
using ApiClinica.Services;

namespace ApiClinica.Interfaces;

public interface IMedicoService
{
    Task<Resultado<List<MedicoReadDTO>>> ListarAsync();
    Task<Resultado<MedicoReadDTO>> BuscarPorIdAsync(int id);
    Task<Resultado<MedicoReadDTO>> CriarAsync(MedicoCreateDTO dto);
    Task<Resultado> AtualizarAsync(int id, MedicoUpdateDTO dto);
    Task<Resultado> ExcluirAsync(int id);
}