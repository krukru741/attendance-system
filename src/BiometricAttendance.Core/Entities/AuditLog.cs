namespace BiometricAttendance.Core.Entities;

/// <summary>
/// Immutable audit trail entry. Records every security-sensitive action.
/// OldValue/NewValue store serialized snapshots for change traceability.
/// </summary>
public class AuditLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty;    // e.g. LOGIN, CORRECTION_APPROVED
    public string Module { get; set; } = string.Empty;    // e.g. Attendance, Employee
    public string? EntityName { get; set; }               // e.g. AttendanceRecord
    public string? EntityId { get; set; }
    public string? OldValue { get; set; }                 // JSON serialized
    public string? NewValue { get; set; }                 // JSON serialized
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? MachineName { get; set; }

    // Navigation
    public User? User { get; set; }
}
