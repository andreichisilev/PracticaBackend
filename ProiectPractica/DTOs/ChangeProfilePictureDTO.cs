using System.ComponentModel.DataAnnotations;

namespace ProiectPractica.DTOs
{
    public class ChangeProfilePictureDTO
    {
        [Url]
        public string? profilePictureUrl { get; set; }
    }
}
