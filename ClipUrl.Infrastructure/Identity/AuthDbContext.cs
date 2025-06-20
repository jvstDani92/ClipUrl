using ClipUrl.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClipUrl.Infrastructure.Identity
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {

        }

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("users");

            builder.Entity<ApplicationUser>()
                .Property(u => u.DisplayName)
                .HasMaxLength(100);

            builder.Entity<RefreshToken>(configuration =>
            {
                configuration.HasKey(x => x.Id);
                configuration.HasIndex(x => x.TokenHash).IsUnique();
                configuration.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
                configuration.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
