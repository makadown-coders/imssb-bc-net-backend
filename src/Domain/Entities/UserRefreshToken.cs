namespace Domain.Entities;

public sealed class UserRefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresUtc { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAtUtc { get; set; }
    public User? User { get; set; }

    public bool IsActive(DateTime utcNow) => !IsRevoked && ExpiresUtc > utcNow;

    public void Revoke(DateTime utcNow)
    {
        IsRevoked = true;
        RevokedAtUtc = utcNow;
    }
}
