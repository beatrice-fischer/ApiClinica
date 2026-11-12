using System.ComponentModel.DataAnnotations;

namespace ApiClinica.DTOs
{
    public class UsuarioUpdateDTO
    {
        public string? Login { get; set; }

        public string? Senha { get; set; }

        public string? Perfil { get; set; }
    }
}
