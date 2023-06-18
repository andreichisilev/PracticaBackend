using System.ComponentModel.DataAnnotations;

namespace ProiectPractica.DTOs
{
    public class LoginDTO
    {
        [EmailAddress]
        public string Email { get; set; }
        [MinLength(3)]
        [MaxLength(32)]
        public string Password { get; set; }
    }
}
