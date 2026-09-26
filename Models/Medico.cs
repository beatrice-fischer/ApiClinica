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

/*POST {
    "Nome": "Teste01",
    "Email": "teste01@gmail.com",
    "Telefone": "(47)988888888",
    "CRM": "113.111.111-11"
}

/*PUT {
    "Id": 1,
    "Nome": "Teste01",
    "Email": "teste01@gmail.com",
    "Telefone": "(47)988888888",
    "CRM": "311.111.111-11"
}*/