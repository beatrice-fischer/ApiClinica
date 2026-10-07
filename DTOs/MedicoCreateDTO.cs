using System.ComponentModel.DataAnnotations;

public class MedicoCreateDTO
{
    public required string Nome { get; set; }
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public required string Email { get; set; }
    public required string Telefone { get; set; }
    public required string CRM { get; set; }
}