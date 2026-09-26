using System.ComponentModel.DataAnnotations;

namespace ApiClinica.Models;

public class Consulta
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public int MedicoId { get; set; }
    public required DateTime Data { get; set; }
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