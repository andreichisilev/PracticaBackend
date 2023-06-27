using System.Text.Json.Serialization;

namespace ProiectPractica.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public DateTime DatePosted { get; set; }
        public string Content { get; set; }
        public string PictureURL { get; set; }
        [JsonIgnore]    
        public User? User { get; set; }
        public int? UserId { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Reaction> Reactions { get; set; }
    }
}
