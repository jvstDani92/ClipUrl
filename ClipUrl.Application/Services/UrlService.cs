using ClipUrl.Domain.Entities;
using ClipUrl.Domain.Interfaces;

namespace ClipUrl.Application.Services
{
    public class UrlService : IUrlService
    {
        private readonly IRepository<ShortUrl> _shortUrlRepository;

        public UrlService(
            IRepository<ShortUrl> shortUrlRepository
            )
        {
            _shortUrlRepository = shortUrlRepository;
        }

        /// <summary>
        /// Creates a short URL for the given original URL.
        /// </summary>
        /// <param name="originalUrl">Long url that have to be shorted</param>
        /// <param name="userId">User ID of the request</param>
        /// <param name="expiresAtUtc">DateTime of expiration</param>
        /// <returns>Unique hash</returns>
        /// <exception cref="ArgumentException">Original URL can´t be null</exception>
        public async Task<string> CreateShortUrlAsync(string originalUrl, Guid? userId = null, DateTime? expiresAtUtc = null)
        {
            if (string.IsNullOrWhiteSpace(originalUrl))
                throw new ArgumentException("Original URL cannot be null or empty.", nameof(originalUrl));

            if (!expiresAtUtc.HasValue)
                expiresAtUtc = DateTime.UtcNow.AddDays(30); // Default expiration date is 30 days from now.

            if (expiresAtUtc.HasValue && expiresAtUtc.Value < DateTime.UtcNow)
                throw new ArgumentException("Expiration date cannot be in the past.", nameof(expiresAtUtc));

            var hash = await GenerateUniqueHashAsync();

            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                Hash = hash,
                UserId = userId,
                ExpiresAtUtc = expiresAtUtc
            };

            await _shortUrlRepository.AddAsync(shortUrl);
            await _shortUrlRepository.SaveChangesAsync();

            return hash;
        }

        public async Task<bool> DeleteShortUrlAsync(string shortUrl)
        {
            if (string.IsNullOrWhiteSpace(shortUrl))
                throw new ArgumentException("Short URL cannot be null or empty.", nameof(shortUrl));

            var shortUrlEntity = await _shortUrlRepository.GetEntityAsync(x => x.Hash.Equals(shortUrl));

            if (shortUrlEntity is null)
                return false;

            _shortUrlRepository.Delete(shortUrlEntity);
            await _shortUrlRepository.SaveChangesAsync();

            return true;
        }

        public Task<IEnumerable<string>> GetAllShortUrlsAsync(Guid? userId = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetOriginalUrlAsync(string shortUrl)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateShortUrlAsync(string shortUrl, string newOriginalUrl, DateTime? newExpiresAtUtc = null)
        {
            throw new NotImplementedException();
        }

        #region Private methods 

        /// <summary>
        /// Generates a unique hash for the short URL.
        /// </summary>
        /// <returns>Unique Hash</returns>
        private async Task<string> GenerateUniqueHashAsync()
        {
            string hash;
            do
            {
                hash = Guid.NewGuid().ToString("N").Substring(0, 8); //Generate a Guid, convert to string of 32 chars and take the first 8.
            } while (await _shortUrlRepository.GetEntityAsync(x => x.Hash.Equals(hash)) != null);
            return hash;
        }

        #endregion 
    }
}
