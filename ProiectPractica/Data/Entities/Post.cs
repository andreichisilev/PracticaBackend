namespace ProiectPractica.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public DateTime DatePosted { get; set; }
        public string Content { get; set; }
        public string PictureURL { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Reaction> Likes { get; set; }
    }
}
