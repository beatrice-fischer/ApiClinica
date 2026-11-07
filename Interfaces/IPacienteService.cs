using ApiClinica.DTOs;
using ApiClinica.Services;

namespace ApiClinica.Interfaces;

public interface IPacienteService
{
    Task<Resultado<List<PacienteReadDTO>>> ListarAsync();
    Task<Resultado<PacienteReadDTO>> BuscarPorIdAsync(int id);
    Task<Resultado<PacienteReadDTO>> CriarAsync(PacienteCreateDTO dto);
    Task<Resultado<PacienteReadDTO>> AtualizarAsync(int id, PacienteUpdateDTO dto);
    Task<Resultado> ExcluirAsync(int id);
}