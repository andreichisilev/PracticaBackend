﻿namespace ProiectPractica.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string HashedPassword { get; set; }
        public string ProfilePictureURL { get; set; }
        public DateTime AccountCreated { get; set; }
        public ICollection<Post> Posts { get; set; }

    }
}
