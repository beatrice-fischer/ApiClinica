using System.ComponentModel.DataAnnotations;

public class PacienteUpdateDTO
{
    public string? Nome { get; set; }
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public DateOnly? DataNasc { get; set; }
}