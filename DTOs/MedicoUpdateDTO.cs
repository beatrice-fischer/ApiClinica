using System.ComponentModel.DataAnnotations;

public class MedicoUpdateDTO
{
    public string? Nome { get; set; }
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? CRM { get; set; }
}