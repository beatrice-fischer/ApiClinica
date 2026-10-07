using System.ComponentModel.DataAnnotations;

public class PacienteCreateDTO
{
    public required string Nome { get; set; }
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public required string Email { get; set; }
    public required string Telefone { get; set; }
    public required DateOnly DataNasc { get; set; }
    public required string Cpf { get; set; }
}