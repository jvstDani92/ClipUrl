using ClipUrl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClipUrl.Infrastructure.Configurations
{
    public sealed class ShortUrlConfiguration : IEntityTypeConfiguration<ShortUrl>
    {
        public void Configure(EntityTypeBuilder<ShortUrl> b)
        {
            b.HasKey(s => s.Id);
        }
    }
}
