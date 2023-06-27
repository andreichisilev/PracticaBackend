using System.ComponentModel.DataAnnotations;

namespace ProiectPractica.DTOs
{
    public class CommentDTO
    {
        [MinLength(1)]
        [MaxLength(500)]
        public string Content { get; set; }
    }
}
