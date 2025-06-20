using ClipUrl.Application.Services;
using ClipUrl.Application.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClipUrl.Api.Controllers
{
    [Authorize(Policy = "RequireUser")]
    [ApiController]
    [Route("api/urls")]
    public class UrlController : ControllerBase
    {
        private readonly IUrlService _urlService;
        private readonly IJwtProvider _jwtProvider;

        public UrlController(
            IUrlService urlService,
            IJwtProvider jwtProvider
            )
        {
            _urlService = urlService;
            _jwtProvider = jwtProvider;
        }

        [HttpGet("{hash}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCompleteUrl(string hash, CancellationToken ct)
        {
            try
            {
                var completeUrl = await _urlService.GetOriginalUrlAsync(hash, ct);

                if (completeUrl is null)
                    return NotFound();

                return Ok(completeUrl);
            }
            catch (ArgumentNullException nullEx)
            {
                return BadRequest(nullEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> InsertUrl([FromBody] string url, CancellationToken ct)
        {
            try
            {
                var userId = await _jwtProvider.GetUserIdFromToken();

                var shortUrl = await _urlService.CreateShortUrlAsync(url, ct, userId: userId);
                return CreatedAtAction(nameof(GetCompleteUrl), new { hash = shortUrl }, shortUrl);
            }
            catch (ArgumentNullException nullEx)
            {
                return BadRequest(nullEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
