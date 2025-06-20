using ClipUrl.Domain.Constants;
using ClipUrl.Domain.Entities.Identity;
using ClipUrl.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace ClipUrl.Api.Configuration
{
    public static class AuthConfiguration
    {
        public static IServiceCollection AddIdentityAndJwtAuth(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddIdentityCore<ApplicationUser>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = true;

                opt.User.RequireUniqueEmail = true;

                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                opt.Lockout.AllowedForNewUsers = true;

            })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AuthDbContext>();

            var jwt = configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidIssuer = jwt["Issuer"],
                        ValidAudience = jwt["Audience"],
                        IssuerSigningKey = key,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("RequireAdmin", p =>
                {
                    p.RequireRole("Admin");
                });

                opts.AddPolicy("RequireUser", p =>
                {
                    p.RequireRole(Roles.Admin, Roles.User);
                });
            });

            return services;
        }
    }
}
