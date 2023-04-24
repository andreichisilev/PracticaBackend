namespace ProiectPractica.Data.Entities
{
    public class Post
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public DateTime DatePosted { get; set; }
        public string Content { get; set; }
        public User User { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
