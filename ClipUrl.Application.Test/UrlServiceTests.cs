using ClipUrl.Application.Services;
using ClipUrl.Domain.Entities;
using ClipUrl.Domain.Interfaces;
using Moq;

namespace ClipUrl.Application.Test
{
    public class UrlServiceTests
    {
        private readonly Mock<IRepository<ShortUrl>> _shortUrlRepositoryMock;
        private readonly UrlService _service;

        public UrlServiceTests()
        {
            _shortUrlRepositoryMock = new Mock<IRepository<ShortUrl>>();
            _service = new UrlService(_shortUrlRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateShortUrlAsync_WithValidUrl_ReturnHash()
        {
            var originalUrl = "https://example.com";

            _shortUrlRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.CreateShortUrlAsync(originalUrl);

            Assert.False(string.IsNullOrEmpty(result));
            _shortUrlRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ShortUrl>()), Times.Once);
        }


        [Fact]
        public async Task GetOriginalUrlAsync_WithExistingHash_ReturnsUrl()
        {
            var existingUrl = new ShortUrl
            {
                Hash = "abc1234",
                OriginalUrl = "https://Urlexample.com",
            };

            _shortUrlRepositoryMock
                .Setup(repo => repo.GetEntityAsync(z => z.Hash.Equals("abc1234")))
                .ReturnsAsync(existingUrl);

            var result = await _service.GetOriginalUrlAsync("abc1234");

            Assert.Equal(existingUrl.OriginalUrl, result);
        }
    }
}