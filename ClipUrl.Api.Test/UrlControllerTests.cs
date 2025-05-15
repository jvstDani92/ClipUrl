using ClipUrl.Api.Controllers;
using ClipUrl.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ClipUrl.Api.Test
{
    public class UrlControllerTests
    {
        private readonly Mock<IUrlService> _urlServiceMock;
        private readonly UrlController _controller;

        public UrlControllerTests()
        {
            _urlServiceMock = new Mock<IUrlService>();
            _controller = new UrlController(_urlServiceMock.Object);
        }

        [Fact]
        public async Task GetCompleteUrl_ValidHash_ReturnsOkWithUrl()
        {
            var hash = "abc1234";
            var expectedUrl = "https://example.com";

            _urlServiceMock
                .Setup(service => service.GetOriginalUrlAsync(hash))
                .ReturnsAsync(expectedUrl);

            var result = await _controller.GetCompleteUrl(hash) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedUrl, result.Value);
        }

        [Fact]
        public async Task InsertUrl_ValidUrl_ReturnsCreated()
        {
            var originalUrl = "https://example.com";
            var expectedHash = "qwerty12";

            _urlServiceMock
                .Setup(svc => svc.CreateShortUrlAsync(originalUrl, null, null))
                .ReturnsAsync(expectedHash);

            var result = await _controller.InsertUrl(originalUrl) as CreatedAtActionResult;

            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.Equal(nameof(UrlController.GetCompleteUrl), result.ActionName);
            Assert.Equal(expectedHash, result.Value);
        }
    }
}