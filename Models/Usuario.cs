using ApiClinica.Services;
using System;

namespace ApiClinica.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;

        public string Perfil { get; set; } = "usuario";

    }
}
