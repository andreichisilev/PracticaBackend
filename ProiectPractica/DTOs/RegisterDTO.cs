using ProiectPractica.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProiectPractica.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        
        [Url]
        public string? ProfilePictureURL { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(32)]
        public string Password { get; set; }

    }
}
