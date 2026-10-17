using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Interfaces
{
	public interface IMedicoMapper
	{
        void ApplyUpdate(MedicoUpdateDTO dto, Medico existente);
        Medico ToEntity(MedicoCreateDTO dto);
    	MedicoReadDTO ToReadDTO(Medico medico);
	}
}
