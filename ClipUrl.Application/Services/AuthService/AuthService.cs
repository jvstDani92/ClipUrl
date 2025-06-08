using ClipUrl.Application.Dtos.Auth;
using ClipUrl.Domain.Entities.Identity;
using ClipUrl.Infrastructure.Auth;
using ClipUrl.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ClipUrl.Application.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly AuthDbContext _authDb;
        private readonly JwtOptions _jwtOptions;

        public AuthService(
           UserManager<ApplicationUser> user,
           AuthDbContext authDbContext,
           JwtOptions jwtOptions
            )
        {
            _users = user;
            _authDb = authDbContext;
            _jwtOptions = jwtOptions;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct)
        {
            var user = await _users.FindByEmailAsync(request.Email)
                ?? throw new InvalidOperationException("Invalid Credentials.");

            if (!await _users.CheckPasswordAsync(user, request.Password))
                throw new InvalidOperationException("Invalid Credentials.");

            return await CreateTokenAsync(user, ct);
        }

        public Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        private async Task<AuthResponseDto> CreateTokenAsync(ApplicationUser user, CancellationToken ct)
        {
            var accessExp = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessMins);
            var accessJwt = BuildJwt(user, accessExp);

            var refresh = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            //TODO: Dradomirov terminar método
            return null;
        }

        private string BuildJwt(ApplicationUser user, DateTime exp)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                expires: exp,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
