namespace BiometricAttendance.Core.Entities;

/// <summary>
/// Named permission token. Permissions are never deleted — only roles lose them.
/// Naming convention: module.action (e.g. employee.create, attendance.kiosk).
/// </summary>
public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;       // e.g. "employee.create"
    public string DisplayName { get; set; } = string.Empty; // e.g. "Create Employee"
    public string Module { get; set; } = string.Empty;      // e.g. "Employee"
    public string Description { get; set; } = string.Empty;

    // Navigation
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
