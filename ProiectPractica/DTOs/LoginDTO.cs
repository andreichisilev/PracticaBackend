using System.ComponentModel.DataAnnotations;

namespace ProiectPractica.DTOs
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(32)]
        public string Password { get; set; }
    }
}
