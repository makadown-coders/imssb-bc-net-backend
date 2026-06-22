namespace Domain.Entities;

public sealed class Role
{
    public string Code { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
