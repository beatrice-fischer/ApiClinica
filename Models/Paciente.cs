using System.ComponentModel.DataAnnotations;
namespace ApiClinica.Models;

public class Paciente
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    [EmailAddress(ErrorMessage = "Email inválido")]
    public required string Email { get; set; }
    public required string Telefone { get; set; }
    public required DateOnly DataNasc { get; set; }
    public required string Cpf { get; set; }
}

/*POST {
    "Nome": "Bea",
    "Email": "bea@gmail.com",
    "Telefone": "(47)988888888",
    "DataNasc": "2000-03-24",
    "Cpf": "111.111.111-11"
}

/*PUT {
    "Id": 1,
    "Nome": "Bea",
    "Email": "bea@gmail.com",
    "Telefone": "(47)988888888",
    "DataNasc": "2000-03-24",
    "Cpf": "211.111.111-11"
}*/