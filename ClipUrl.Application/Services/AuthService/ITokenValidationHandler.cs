using ClipUrl.Application.Dtos.Auth;
using ClipUrl.Application.Enums.Auth;

namespace ClipUrl.Application.Services.AuthService
{
    public interface ITokenValidationHandler
    {
        Task<ValidationResultDto> ValidateAsync(string token, TokenType tokenType, SocialNetwork socialNetwork, CancellationToken ct);
    }
}
