using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClipUrl.Infrastructure.Identity
{
    public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            const string conn = "Server=ASUS-TUF\\SQLEXPRESS; DataBase=AuthClipUrlDb; Trusted_Connection=True; MultipleActiveResultSets=True; TrustServerCertificate=True;";

            var options = new DbContextOptionsBuilder<AuthDbContext>()
                .UseSqlServer(conn)
                .Options;

            return new AuthDbContext(options);
        }
    }
}
