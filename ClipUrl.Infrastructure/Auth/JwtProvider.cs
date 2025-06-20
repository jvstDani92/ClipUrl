using ClipUrl.Application.Services.AuthService;
using ClipUrl.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ClipUrl.Infrastructure.Auth
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _options;
        private readonly IHttpContextAccessor _httpContextAccesor;

        public JwtProvider(
            IOptions<JwtOptions> options,
            IHttpContextAccessor httpContextAccesor
            )
        {
            _options = options.Value;
            httpContextAccesor = _httpContextAccesor;
        }


        public Task<string> CreateAccessTokenAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(BuildJwt(user, DateTime.UtcNow.AddMinutes(_options.AccessMins)));


        public Task<string> CreateRefreshTokenAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)));

        public Task<string?> GetClaim(string claim, CancellationToken ct)
        {
            var token = _httpContextAccesor.HttpContext?.Request
                .Headers["Authorization"]
                .FirstOrDefault()?
                .Split(" ")
                .Last();

            if (string.IsNullOrWhiteSpace(token))
                return Task.FromResult<string?>(null);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var claimValue = jwt.Claims.FirstOrDefault(c => c.Type.Equals(claim, StringComparison.OrdinalIgnoreCase))?.Value;

            return Task.FromResult(claimValue);
        }

        public Task<Guid?> GetUserIdFromToken()
        {
            var token = _httpContextAccesor.HttpContext?.Request
                .Headers["Authorization"]
                .FirstOrDefault()?
                .Split(" ")
                .Last();

            if (string.IsNullOrWhiteSpace(token))
                return Task.FromResult<Guid?>(null);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
                return Task.FromResult<Guid?>(userId);

            return Task.FromResult<Guid?>(null);
        }
        private string BuildJwt(ApplicationUser user, DateTime exp)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityTokenHandler()
                .WriteToken(new JwtSecurityToken(_options.Issuer, _options.Audience, claims, expires: exp, signingCredentials: creds));
        }
    }
}
