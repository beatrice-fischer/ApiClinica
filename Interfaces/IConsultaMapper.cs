using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Interfaces
{
	public interface IConsultaMapper
	{
        void ApplyUpdate(ConsultaUpdateDTO dto, Consulta existente);
        Consulta ToEntity(ConsultaCreateDTO dto);
    	ConsultaReadDTO ToReadDTO(Consulta consulta);
	}
}
