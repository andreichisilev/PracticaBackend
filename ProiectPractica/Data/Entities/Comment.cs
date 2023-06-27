using System.Text.Json.Serialization;

namespace ProiectPractica.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime DateCommented { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
        public int? UserId { get; set; }
        [JsonIgnore]
        public Post? Post { get; set; }
        public int? PostId { get; set; }
    }
}
