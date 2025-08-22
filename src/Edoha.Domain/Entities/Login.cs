namespace Edoha.Domain.Entities
{
    public class Login
    {
        public Guid Id { get; set; } = Guid.NewGuid();  // Pode ser gerado no código ou pelo DB
        public Guid IdUser { get; set; }
        public string RefreshTokenHash { get; set; }
        public string Ip { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool Revoked { get; set; }
        public DateTime? RevokedAt { get; set; }
    }
}
