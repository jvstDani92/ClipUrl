namespace ClipUrl.Application.Dtos.Auth
{
    public record AuthResponseDto(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresUtc
        );
}
