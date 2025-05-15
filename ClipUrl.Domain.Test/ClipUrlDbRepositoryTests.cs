using ClipUrl.Domain.Entities;
using ClipUrl.Infrastructure.Data;
using ClipUrl.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ClipUrl.Infrastructure.Test
{
    public class ClipUrlDbRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ThenRetrieveEntity()
        {
            var options = new DbContextOptionsBuilder<ClipUrlDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using var context = new ClipUrlDbContext(options);
            var repository = new ClipUrlDbRepository<ShortUrl>(context);
            var shortUrl = new ShortUrl
            {
                Hash = "test1234",
                OriginalUrl = "https://testExample.com"
            };

            await repository.AddAsync(shortUrl);
            await repository.SaveChangesAsync();

            var retrieved = await repository.GetEntityAsync(x => x.Hash == "test1234");

            Assert.NotNull(retrieved);
            Assert.Equal("https://testExample.com", retrieved.OriginalUrl);
        }
    }
}