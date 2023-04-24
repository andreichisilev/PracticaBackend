namespace ProiectPractica.Data.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string HashedPassword { get; set; }

        public ICollection<Post> Posts { get; set; }

    }
}
