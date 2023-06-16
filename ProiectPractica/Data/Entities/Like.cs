namespace ProiectPractica.Data.Entities
{
    public class Like
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string LikeType { get; set; }

    }
}
