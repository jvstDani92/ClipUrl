using ClipUrl.Domain.Entities.Identity;

namespace ClipUrl.Application.Services.AuthService
{
    public interface IJwtProvider
    {
        Task<string> CreateAccessTokenAsync(ApplicationUser user, CancellationToken ct);

        Task<string> CreateRefreshTokenAsync(ApplicationUser user, CancellationToken ct);

        Task<string?> GetClaim(string claim, CancellationToken ct);

        Task<Guid?> GetUserIdFromToken();
    }
}
