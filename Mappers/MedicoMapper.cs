using ApiClinica.Models;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;

namespace ApiClinica.Mappers;

public class MedicoMapper : IMedicoMapper
{
    public Medico ToEntity(MedicoCreateDTO dto)
    {
        return new Medico
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            CRM = dto.CRM
        };
    }

    public MedicoReadDTO ToReadDTO(Medico medico)
    {
        return new MedicoReadDTO
        {
            Id = medico.Id,
            Nome = medico.Nome,
            Email = medico.Email,
            Telefone = medico.Telefone,
            CRM = medico.CRM
        };
    }
    public void ApplyUpdate (MedicoUpdateDTO dto, Medico entidade)
    {
        if (dto.Nome is not null) entidade.Nome = dto.Nome;
        if (dto.Email is not null) entidade.Email = dto.Email;
        if (dto.Telefone is not null) entidade.Telefone = dto.Telefone;
        if (dto.CRM is not null) entidade.CRM = dto.CRM;
    }
}