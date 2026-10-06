using ApiClinica.Models;

namespace ApiClinica.Mappers;

public static class MedicoMapper
{
    public static MedicoCreateDTO ToEntity(MedicoCreateDTO dto)
    {
        return new MedicoCreateDTO
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            CRM = dto.CRM,
        };
    }

    public static MedicoReadDTO toReadDTO(Medico medico)
    {
        return new MedicoReadDTO
        {
            Id = medico.Id,
            Nome = medico.Nome,
            Email = medico.Email,
            Telefone = medico.Telefone,
            CRM = medico.CRM,
        };
    }
    public static void ApplyUpdate (MedicoUpdateDTO dto, Medico entidade)
    {
        if (dto.Nome is not null) entidade.Nome = dto.Nome;
        if (dto.Email is not null) entidade.Email = dto.Email;
        if (dto.Telefone is not null) entidade.Telefone = dto.Telefone;
        if (dto.CRM is not null) entidade.CRM = dto.CRM;
    }
}