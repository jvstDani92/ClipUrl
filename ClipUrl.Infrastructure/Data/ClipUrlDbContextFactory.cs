using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClipUrl.Infrastructure.Data
{
    public class ClipUrlDbContextFactory : IDesignTimeDbContextFactory<ClipUrlDbContext>
    {
        public ClipUrlDbContext CreateDbContext(string[] args)
        {
            const string conn = "Server=ASUS-TUF\\SQLEXPRESS; DataBase=ClipUrlDb; Trusted_Connection=True; MultipleActiveResultSets=True; TrustServerCertificate=True;";

            var options = new DbContextOptionsBuilder<ClipUrlDbContext>()
                .UseSqlServer(conn)
                .Options;

            return new ClipUrlDbContext(options);
        }
    }

}
