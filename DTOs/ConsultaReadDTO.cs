public class ConsultaReadDTO
{
    public required int Id { get; set; }
    public required int PacienteId { get; set; }
    public required int MedicoId { get; set; }
    public required DateTime Data { get; set; }
}