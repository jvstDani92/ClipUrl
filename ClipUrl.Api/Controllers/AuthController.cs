using ClipUrl.Application.Dtos.Auth;
using ClipUrl.Application.Exceptions.Auth;
using ClipUrl.Application.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClipUrl.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request, CancellationToken ct)
        {
            try
            {
                if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest("Email and password cannot be null, empty, or whitespace.");

                var tokens = await _auth.RegisterAsync(request, ct);

                if (tokens is null)
                    return BadRequest("Registration failed. Please try again.");

                return Ok(tokens);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestDto request, CancellationToken ct)
        {
            try
            {
                if (request is null)
                    return BadRequest($"{nameof(request)} cannot be null.");

                var tokens = await _auth.LoginAsync(request, ct);

                if (tokens is null)
                    return Unauthorized("Invalid credentials.");

                return Ok(tokens);
            }
            catch (JwtSecurityCheckException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] string token, CancellationToken ct)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return BadRequest($"{nameof(token)} cannot be null.");

                var tokens = await _auth.RefreshTokenAsync(token, ct);

                if (tokens is null)
                    return Unauthorized("Invalid or expired refresh token.");

                return Ok(tokens);
            }
            catch (JwtSecurityCheckException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
