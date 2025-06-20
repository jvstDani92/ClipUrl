using Microsoft.AspNetCore.Identity;

namespace ClipUrl.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? DisplayName { get; set; }
    }
}
