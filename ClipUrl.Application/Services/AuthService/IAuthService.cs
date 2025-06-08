using ClipUrl.Application.Dtos.Auth;

namespace ClipUrl.Application.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct);

        Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct);

        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken ct);
    }
}
