using System.ComponentModel.DataAnnotations;

public class PacienteCreateDTO
{
    public required string Nome { get; set; }
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public required string Email { get; set; }
    public required string Telefone { get; set; }
    //Conferir no Controller lógica de telefone, não podemos utilizar atributo [Phone]
    public required DateOnly DataNasc { get; set; }
    public required string Cpf { get; set; }
}