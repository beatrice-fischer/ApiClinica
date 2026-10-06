using ApiClinica.Models;

namespace ApiClinica.Mappers;

public static class ConsultaMapper
{
    public static ConsultaCreateDTO ToEntity(ConsultaCreateDTO dto)
    {
        return new ConsultaCreateDTO
        {
            PacienteId = dto.PacienteId,
            MedicoId = dto.MedicoId,
            DataHora = dto.DataHora
        };
    }

    public static ConsultaReadDTO ToReadDTO(Consulta c)
    {
        return new ConsultaReadDTO
        {
            Id = c.Id,
            PacienteId = c.PacienteId,
            MedicoId = c.MedicoId,
            DataHora = c.DataHora
        };
    }

    public static void ApplyUpdate(ConsultaUpdateDTO dto, Consulta c)
    {
        if (dto.PacienteId is not null) c.PacienteId = dto.PacienteId.Value;
        if (dto.MedicoId is not null) c.MedicoId = dto.MedicoId.Value;
        if (dto.DataHora is not null) c.DataHora = dto.DataHora.Value;

    }
}