namespace BiometricAttendance.Core.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>Soft-delete flag — no lifecycle semantics, just active/removed.</summary>
    public bool IsActive { get; set; } = true;

    public bool IsSystem { get; set; } = false; // System roles cannot be deleted
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
