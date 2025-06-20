using ClipUrl.Domain.Constants;
using ClipUrl.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClipUrl.Infrastructure.Identity
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            bool seedDevAdmin = false
            )
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            foreach (var role in Roles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }

            if (!seedDevAdmin)
                return;

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var seedAdmin = configuration.GetSection("SeedAdmin");
            string adminEmail = seedAdmin["Email"];
            string adminPassword = seedAdmin["Password"];

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    DisplayName = "Dev Admin"
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (!result.Succeeded)
                    throw new InvalidOperationException($"Admin user not created: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            if (!await userManager.IsInRoleAsync(admin, Roles.Admin))
                await userManager.AddToRoleAsync(admin, Roles.Admin);
        }
    }
}
