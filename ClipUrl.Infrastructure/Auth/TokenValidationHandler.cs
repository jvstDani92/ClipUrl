using ClipUrl.Application.Dtos.Auth;
using ClipUrl.Application.Enums.Auth;
using ClipUrl.Application.Services.AuthService;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ClipUrl.Infrastructure.Auth
{
    public class TokenValidationHandler : ITokenValidationHandler
    {
        private readonly JwtOptions _options;

        public TokenValidationHandler(IOptions<JwtOptions> options)
            => _options = options.Value;

        public Task<ValidationResultDto> ValidateAsync(string token, TokenType tokenType, SocialNetwork socialNetwork, CancellationToken ct)
        {
            if (socialNetwork != SocialNetwork.None)
                return Task.FromResult(new ValidationResultDto(false, null, "External token not supported", socialNetwork, tokenType));

            var handler = new JwtSecurityTokenHandler();

            try
            {
                handler.ValidateToken(token, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = _options.Issuer,
                    ValidAudience = _options.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                }, out var validated);

                var jwt = (JwtSecurityToken)validated;
                var userId = jwt.Subject;

                return Task.FromResult(new ValidationResultDto(true, userId, null, socialNetwork, tokenType));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ValidationResultDto(false, null, ex.Message, socialNetwork, tokenType));
            }
        }
    }
}
