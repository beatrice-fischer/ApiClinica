using ApiClinica.DTOs;
using ApiClinica.Services;
namespace ApiClinica.Interfaces;
public interface IConsultaService
{
    Task<Resultado<List<ConsultaReadDTO>>> ListarAsync();
    Task<Resultado<ConsultaReadDTO>> BuscarPorIdAsync(int id);
    Task<Resultado<ConsultaReadDTO>> CriarAsync(ConsultaCreateDTO dto);
    Task<Resultado> AtualizarAsync(int id, ConsultaUpdateDTO dto);
    Task<Resultado> ExcluirAsync(int id);
}