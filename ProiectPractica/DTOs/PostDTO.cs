using ProiectPractica.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProiectPractica.DTOs
{
    public class PostDTO
    {
        [Required]
        [MaxLength(2200)]
        public string Content { get; set; }
        [Required]
        [Url]
        public string PictureURL { get; set; }
     
    }
}
