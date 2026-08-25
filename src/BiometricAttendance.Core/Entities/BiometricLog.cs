using BiometricAttendance.Core.Enums;

namespace BiometricAttendance.Core.Entities;

/// <summary>
/// Records raw device-level activity (connections, syncs, pushes, pulls).
/// Kept separate from AttendanceEvent to preserve device diagnostics without
/// contaminating attendance history (per 05-BIOMETRICS.md Section 6.2).
/// </summary>
public class BiometricLog
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public BiometricLogType LogType { get; set; }
    public string? Message { get; set; }
    public string? RawPayload { get; set; }
    public bool Success { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public BiometricDevice Device { get; set; } = null!;
}
