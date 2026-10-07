using System.ComponentModel.DataAnnotations;

namespace ApiClinica.Models;

public class Consulta
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public int MedicoId { get; set; }
    public required DateTime DataHora { get; set; }
}