using System;

namespace ApiClinica.DTOs
{
    public class UsuarioReadDTO
    {
        public int Id { get; set; } = default!;

        public string Login { get; set; } = default!;

        public string Senha { get; set; } = default!;

        public string Perfil { get; set; } = default!;
    }
}