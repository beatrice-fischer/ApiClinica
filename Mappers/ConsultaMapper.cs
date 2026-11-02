using ApiClinica.Models;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;

namespace ApiClinica.Mappers;

public class ConsultaMapper : IConsultaMapper
{
    public Consulta ToEntity(ConsultaCreateDTO dto)
    {
        return new Consulta
        {
            PacienteId = dto.PacienteId,
            MedicoId = dto.MedicoId,
            DataHora = dto.DataHora
        };
    }

    public ConsultaReadDTO ToReadDTO(Consulta consulta)
    {
        return new ConsultaReadDTO
        {
            Id = consulta.Id,
            PacienteId = consulta.PacienteId,
            MedicoId = consulta.MedicoId,
            DataHora = consulta.DataHora
        };
    }

    public void ApplyUpdate(ConsultaUpdateDTO dto, Consulta entidade)
    {
        if (dto.PacienteId is not null) entidade.PacienteId = dto.PacienteId.Value;
        if (dto.MedicoId is not null) entidade.MedicoId = dto.MedicoId.Value;
        if (dto.DataHora is not null) entidade.DataHora = dto.DataHora.Value;

    }
}