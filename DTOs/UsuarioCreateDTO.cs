using System;

namespace ApiClinica.DTOs
{
    public class UsuarioCreateDTO
    {
        public string Login { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;

        public string? Perfil { get; set; } = "usuario";
    }
}
