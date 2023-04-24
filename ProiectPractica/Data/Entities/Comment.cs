namespace ProiectPractica.Data.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string CommentContent { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
