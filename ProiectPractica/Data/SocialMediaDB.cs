
using Microsoft.EntityFrameworkCore;
using ProiectPractica.Data.Entities;

namespace ProiectPractica.Data
{
    public class SocialMediaDB : DbContext
    {
        public SocialMediaDB(DbContextOptions<SocialMediaDB> options) : base(options)
        { 
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
    }
}
    

