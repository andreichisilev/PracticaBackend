namespace ProiectPractica.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
