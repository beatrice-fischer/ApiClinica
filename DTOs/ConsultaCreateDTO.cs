using System.ComponentModel.DataAnnotations;

public class ConsultaCreateDTO
{
    public required int PacienteId { get; set; }
    public required int MedicoId { get; set; }
    public required DateTime Data { get; set; }
}