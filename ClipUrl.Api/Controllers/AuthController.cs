using ClipUrl.Application.Dtos.Auth;
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
        [AllowAnonymous]
        public Task<AuthResponseDto> Register(RegisterRequestDto request, CancellationToken ct)
            => _auth.RegisterAsync(request, ct);

        [HttpPost("login")]
        [AllowAnonymous]
        public Task<AuthResponseDto> Login(LoginRequestDto request, CancellationToken ct)
            => _auth.LoginAsync(request, ct);


        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public Task<AuthResponseDto> Refresh([FromBody] string token, CancellationToken ct)
            => _auth.RefreshTokenAsync(token, ct);
    }
}
