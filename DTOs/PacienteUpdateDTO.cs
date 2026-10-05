// Função TelephoneNumber e EmailAddress AINDA NÃO EXISTEM precisam ser criadads posteriormente//
using System.ComponentModel.DataAnnotations;

public class PacienteUpdateDTO
{
    public string? Nome { get; set; }
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public DateOnly? DataNasc { get; set; }
    public string? Cpf { get; set; }
}