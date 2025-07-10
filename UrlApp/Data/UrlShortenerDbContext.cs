using Microsoft.EntityFrameworkCore; 
using UrlApp.Models; 

namespace UrlApp.Data
{
    public class UrlShortenerDbContext : DbContext
    {
        public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Url> Urls { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Url>()
                .HasOne(u => u.User)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
