namespace ClipUrl.Infrastructure.Auth
{
    public class JwtOptions
    {
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;

        public string Key { get; init; } = string.Empty;

        public int AccessMins { get; init; } = 15;

        public int RefreshDays { get; init; } = 7;
    }
}
