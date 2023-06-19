using static ProiectPractica.Enums;

namespace ProiectPractica.Data.Entities
{
    
    public class Reaction
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public Post Post { get; set; }
        public int PostId { get; set; }
        public ReactionType Type { get; set; }

    }
}
