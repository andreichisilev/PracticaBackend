
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
        public DbSet<Reaction> Reactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reactions)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            
            base.OnModelCreating(modelBuilder);
        }

    }
}
    

