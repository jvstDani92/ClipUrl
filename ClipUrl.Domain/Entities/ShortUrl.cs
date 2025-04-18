namespace ClipUrl.Domain.Entities
{
    public class ShortUrl
    {
        public Guid Id { get; set; }

        public string Hash { get; set; } = default!;

        public string OriginalUrl { get; set; } = default!;

        public Guid? UserId { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? ExpiresAtUtc { get; set; } = DateTime.UtcNow;

        public int ClickCount { get; set; }
    }
}
