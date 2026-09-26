using ApiClinica.Models;

namespace ApiClinica.Mappers;

public static class PacienteMapper
{
    public static Paciente ToModel(PacienteCreateDTO dto)
    {
        return new Paciente
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            DataNasc = dto.DataNasc,
            Cpf = dto.Cpf
        };
    }

    public static PacienteReadDTO ToDTO(Paciente paciente)
    {
        return new PacienteReadDTO
        {
            Id = paciente.Id,
            Nome = paciente.Nome,
            Email = paciente.Email,
            Telefone = paciente.Telefone,
            DataNasc = paciente.DataNasc
        };
    }
}