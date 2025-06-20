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
        public async Task<string> CreateShortUrlAsync(string originalUrl, CancellationToken ct, Guid? userId = null, DateTime? expiresAtUtc = null)
        {
            if (string.IsNullOrWhiteSpace(originalUrl))
                throw new ArgumentNullException("Original URL cannot be null or empty.", nameof(originalUrl));

            if (!expiresAtUtc.HasValue)
                expiresAtUtc = DateTime.UtcNow.AddDays(30); // Default expiration date is 30 days from now.

            if (expiresAtUtc.HasValue && expiresAtUtc.Value < DateTime.UtcNow)
                throw new ArgumentException("Expiration date cannot be in the past.", nameof(expiresAtUtc));

            try
            {
                var hash = await GenerateUniqueHashAsync();

                var shortUrl = new ShortUrl
                {
                    OriginalUrl = originalUrl,
                    Hash = hash,
                    UserId = userId,
                    CreatedAtUtc = DateTime.UtcNow,
                    ExpiresAtUtc = expiresAtUtc
                };

                await _shortUrlRepository.AddAsync(shortUrl);
                await _shortUrlRepository.SaveChangesAsync();

                return hash;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("An invalid operation occurred while creating the hash URL.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the short URL.", ex);
            }

        }

        /// <summary>
        /// Deletes a short URL from the database.
        /// </summary>
        /// <param name="shortUrl">The hash to delete</param>
        /// <returns>Bool</returns>
        /// <exception cref="ArgumentException">Short Url can´t be null</exception>
        /// <exception cref="DbUpdateException">Exception during the delete command with EF</exception>
        /// <exception cref="Exception">General exception</exception>
        public async Task<bool> DeleteShortUrlAsync(string shortUrl, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(shortUrl))
                throw new ArgumentNullException(nameof(shortUrl), "Short URL cannot be null or empty.");

            try
            {
                var shortUrlEntity = await _shortUrlRepository.GetEntityAsync(x => x.Hash.Equals(shortUrl));
                if (shortUrlEntity is null)
                    return false;

                _shortUrlRepository.Delete(shortUrlEntity);
                await _shortUrlRepository.SaveChangesAsync();

                return true;
            }
            catch (InvalidOperationException invalidOpEx)
            {
                throw new InvalidOperationException("An invalid operation occurred while deleting the short URL.", invalidOpEx);
            }

            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the short URL.", ex);
            }
        }

        public Task<IEnumerable<string>> GetAllShortUrlsAsync(Guid userId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the original URL for a given short URL.
        /// </summary>
        /// <param name="shortUrl">The hash of the URL to retrieve</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The hash can´t be null</exception>
        /// <exception cref="ArgumentException">The entity from database can´t be null</exception>
        /// <exception cref="DbUpdateException">Exception during the delete command with EF</exception>
        /// <exception cref="Exception">General exception</exception>
        public async Task<string> GetOriginalUrlAsync(string shortUrl, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(shortUrl))
                throw new ArgumentNullException(nameof(shortUrl), "Short URL cannot be null or empty.");

            try
            {
                var shortUrlEntity = await _shortUrlRepository.GetEntityAsync(x => x.Hash.Equals(shortUrl));

                if (shortUrlEntity is null)
                    throw new ArgumentException("Short URL not found.", nameof(shortUrl));

                // TODO: Procesar y mapear a DTO, si es necesario.
                return shortUrlEntity.OriginalUrl;
            }
            catch (InvalidOperationException dbEx)
            {
                throw new InvalidOperationException("An error occurred while retrieving the short URL from the database.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the original URL.", ex);
            }
        }

        public Task<bool> UpdateShortUrlAsync(string shortUrl, string newOriginalUrl, CancellationToken ct, DateTime? newExpiresAtUtc = null)
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
