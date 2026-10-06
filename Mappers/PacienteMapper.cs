using ApiClinica.Models;

namespace ApiClinica.Mappers;

public static class PacienteMapper
{
    public static PacienteCreateDTO ToEntity(PacienteCreateDTO dto)
    {
        return new PacienteCreateDTO
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            DataNasc = dto.DataNasc,
            Cpf = dto.Cpf
        };
    }

    public static PacienteReadDTO ToReadDTO(Paciente paciente)
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
    public static void ApplyUpdate (PacienteUpdateDTO dto, Paciente entidade)
    {
        if (dto.Nome is not null) entidade.Nome = dto.Nome;
        if (dto.Email is not null) entidade.Email = dto.Email;
        if (dto.Telefone is not null) entidade.Telefone = dto.Telefone;
        if (dto.DataNasc is not null) entidade.DataNasc = dto.DataNasc.Value;
    }   
}