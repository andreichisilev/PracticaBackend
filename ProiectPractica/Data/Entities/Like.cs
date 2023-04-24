namespace ProiectPractica.Data.Entities
{
    public class Like
    {
        public int LikeId { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string LikeType { get; set; }

    }
}
