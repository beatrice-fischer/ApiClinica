using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Interfaces
{
	public interface IPacienteMapper
	{
        void ApplyUpdate(PacienteUpdateDTO dto, Paciente existente);
        Paciente ToEntity(PacienteCreateDTO dto);
    	PacienteReadDTO ToReadDTO(Paciente paciente);
	}
}
