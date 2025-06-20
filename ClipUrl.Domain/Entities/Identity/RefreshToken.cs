namespace ClipUrl.Domain.Entities.Identity
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string TokenHash { get; set; } = null!;

        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }

        public DateTime? Revoked { get; set; }
    }
}
