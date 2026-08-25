namespace BiometricAttendance.Core.Entities;

/// <summary>
/// Key-value store for system-wide configuration settings.
/// Values are always stored as strings; the application layer parses/validates them.
/// </summary>
public class SystemSetting
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;       // e.g. "attendance.grace_period_minutes"
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }                 // General, Attendance, Biometrics
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int? UpdatedByUserId { get; set; }
}
