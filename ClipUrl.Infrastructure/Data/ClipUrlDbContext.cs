using ClipUrl.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClipUrl.Infrastructure.Data
{
    public class ClipUrlDbContext : DbContext
    {
        public ClipUrlDbContext(DbContextOptions<ClipUrlDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortUrl>()
                .HasKey(s => s.Id);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ShortUrl> ShortUrls { get; set; }
    }
}
