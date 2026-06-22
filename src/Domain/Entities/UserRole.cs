namespace Domain.Entities;

public sealed class UserRole
{
    public Guid UserId { get; set; }
    public string RoleCode { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public Guid? AssignedByUserId { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public User? User { get; set; }
    public User? AssignedByUser { get; set; }
    public Role? Role { get; set; }
}
