// Função TelephoneNumber e EmailAddress AINDA NÃO EXISTEM precisam ser criadads posteriormente//

using System.ComponentModel.DataAnnotations;

public class PacienteCreateDTO
{
    public required string Nome { get; set; }
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public required string Email { get; set; }
    [TelephoneNumber(ErrorMessage = "Numero de telefone inválido.")]
    public required string Telefone { get; set; }
    public required DateOnly DataNasc { get; set; }
    public required string Cpf { get; set; }
}