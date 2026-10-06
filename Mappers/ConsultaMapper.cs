using ApiClinica.Models;

namespace ApiClinica.Mappers;

public static class ConsultaMapper
{
    public static Consulta ToEntity(ConsultaCreateDTO dto)
    {
        return new Consulta
        {
            PacienteId = dto.PacienteId,
            MedicoId = dto.MedicoId,
            DataHora = dto.DataHora
        };
    }

    public static ConsultaReadDTO ToReadDTO(Consulta consulta)
    {
        return new ConsultaReadDTO
        {
            Id = consulta.Id,
            PacienteId = consulta.PacienteId,
            MedicoId = consulta.MedicoId,
            DataHora = consulta.DataHora
        };
    }

    public static void ApplyUpdate(ConsultaUpdateDTO dto, Consulta entidade)
    {
        if (dto.PacienteId is not null) entidade.PacienteId = dto.PacienteId.Value;
        if (dto.MedicoId is not null) entidade.MedicoId = dto.MedicoId.Value;
        if (dto.DataHora is not null) entidade.DataHora = dto.DataHora.Value;

    }
}