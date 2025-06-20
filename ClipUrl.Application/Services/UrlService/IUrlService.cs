namespace ClipUrl.Application.Services
{
    public interface IUrlService
    {
        Task<string> CreateShortUrlAsync(string originalUrl, CancellationToken ct, Guid? userId = null, DateTime? expiresAtUtc = null);

        Task<string> GetOriginalUrlAsync(string shortUrl, CancellationToken ct);

        Task<bool> DeleteShortUrlAsync(string shortUrl, CancellationToken ct);

        Task<bool> UpdateShortUrlAsync(string shortUrl, string newOriginalUrl, CancellationToken ct, DateTime? newExpiresAtUtc = null);

        Task<IEnumerable<string>> GetAllShortUrlsAsync(Guid userId, CancellationToken ct);
    }
}
