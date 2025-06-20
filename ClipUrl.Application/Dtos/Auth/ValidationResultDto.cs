using ClipUrl.Application.Enums.Auth;

namespace ClipUrl.Application.Dtos.Auth
{
    public record ValidationResultDto(
        bool isValid,
        string? UserId,
        string? Reason,
        SocialNetwork Network = SocialNetwork.None,
        TokenType TokenType = TokenType.Access
        );
}
