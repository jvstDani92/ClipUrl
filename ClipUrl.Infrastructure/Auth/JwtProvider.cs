using ClipUrl.Application.Services.AuthService;
using ClipUrl.Domain.Entities.Identity;
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

        public JwtProvider(IOptions<JwtOptions> options) => _options = options.Value;

        public Task<string> CreateAccessTokenAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(BuildJwt(user, DateTime.UtcNow.AddMinutes(_options.AccessMins)));


        public Task<string> CreateRefreshTokenAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)));

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
