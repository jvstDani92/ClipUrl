using ClipUrl.Application.Dtos.Auth;
using ClipUrl.Application.Enums.Auth;
using ClipUrl.Application.Exceptions.Auth;
using ClipUrl.Application.Services.AuthService;
using ClipUrl.Domain.Constants;
using ClipUrl.Domain.Entities.Identity;
using ClipUrl.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace ClipUrl.Infrastructure.Auth
{
    public class JwtAuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly IJwtProvider _jwtProvider;
        private readonly ITokenValidationHandler _validatorHandler;
        private readonly AuthDbContext _authDb;
        private readonly JwtOptions _options;

        public JwtAuthService(
            UserManager<ApplicationUser> users,
            IJwtProvider jwtProvider,
            ITokenValidationHandler validatorHandler,
            AuthDbContext authDb,
            IOptions<JwtOptions> options)
        {
            _users = users;
            _jwtProvider = jwtProvider;
            _validatorHandler = validatorHandler;
            _authDb = authDb;
            _options = options.Value;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct)
        {
            var user = await _users.FindByEmailAsync(request.Email)
                  ?? throw new JwtSecurityCheckException("Invalid credentials.");

            if (!await _users.CheckPasswordAsync(user, request.Password))
                throw new JwtSecurityCheckException("Invalid credentials.");

            return await IssueTokenAsync(user, ct);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken ct)
        {
            var validation = await _validatorHandler.ValidateAsync(
                refreshToken,
                TokenType.Refresh,
                SocialNetwork.None,
                ct);

            if (!validation.isValid)
                throw new JwtSecurityCheckException(validation.Reason ?? "Invalid refresh token.");

            var hash = Convert.ToHexString(SHA256.HashData(Convert.FromBase64String(refreshToken)));
            var stored = await _authDb.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == hash && r.Revoked == null, ct)
                ?? throw new JwtSecurityCheckException("Refresh token not found.");

            if (stored.Expires < DateTime.UtcNow)
                throw new JwtSecurityCheckException("Refresh token expired.");

            stored.Revoked = DateTime.UtcNow;

            var user = await _users.FindByIdAsync(stored.UserId.ToString())
                ?? throw new JwtSecurityCheckException("User not found.");

            return await IssueTokenAsync(user, ct);
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct)
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
            };

            var result = await _users.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

            await _users.AddToRoleAsync(user, Roles.User);
            return await IssueTokenAsync(user, ct);
        }

        private async Task<AuthResponseDto> IssueTokenAsync(ApplicationUser user, CancellationToken ct)
        {
            var access = await _jwtProvider.CreateAccessTokenAsync(user, ct);
            var refresh = await _jwtProvider.CreateRefreshTokenAsync(user, ct);

            _authDb.RefreshTokens.Add(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = Convert.ToHexString(SHA256.HashData(Convert.FromBase64String(refresh))),
                Expires = DateTime.UtcNow.AddDays(_options.RefreshDays),
                Created = DateTime.UtcNow
            });

            await _authDb.SaveChangesAsync(ct);
            return new(access, refresh, DateTime.UtcNow.AddMinutes(_options.AccessMins));
        }
    }
}
