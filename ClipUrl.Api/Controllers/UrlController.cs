using ClipUrl.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClipUrl.Api.Controllers
{
    [ApiController]
    [Route("api/urls")]
    public class UrlController : ControllerBase
    {
        private readonly IUrlService _urlService;

        public UrlController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        [HttpGet("{hash}")]
        public async Task<IActionResult> GetCompleteUrl(string hash)
        {
            try
            {
                var completeUrl = await _urlService.GetOriginalUrlAsync(hash);

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
        public async Task<IActionResult> InsertUrl([FromBody] string url)
        {
            try
            {
                var shortUrl = await _urlService.CreateShortUrlAsync(url);
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
