namespace ClipUrl.Application.Services
{
    public interface IUrlService
    {
        Task<string> CreateShortUrlAsync(string originalUrl, Guid? userId = null, DateTime? expiresAtUtc = null);

        Task<string> GetOriginalUrlAsync(string shortUrl);

        Task<bool> DeleteShortUrlAsync(string shortUrl);

        Task<bool> UpdateShortUrlAsync(string shortUrl, string newOriginalUrl, DateTime? newExpiresAtUtc = null);

        Task<IEnumerable<string>> GetAllShortUrlsAsync(Guid? userId = null);
    }
}
