using System.ComponentModel.DataAnnotations;

namespace ApiClinica.Models;

public class Medico
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    [EmailAddress(ErrorMessage = "Email inválido")]
    public required string Email { get; set; }
    public required string Telefone { get; set; }
    public required string CRM { get; set; }
}