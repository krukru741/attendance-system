using BiometricAttendance.Core.Enums;

namespace BiometricAttendance.Core.Entities;

/// <summary>
/// A raw, immutable biometric scan event. This record must NEVER be modified after creation.
/// Corrections are applied to AttendanceRecord, not here.
/// </summary>
public class AttendanceEvent
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int? DeviceId { get; set; }
    public AttendanceEventType EventType { get; set; }
    public DateTime Timestamp { get; set; }
    public bool BiometricVerified { get; set; } = true;
    public AttendanceEventSource Source { get; set; } = AttendanceEventSource.BiometricDevice;

    /// <summary>
    /// Optional idempotency key for offline-sync duplicate prevention (14-OFFLINE-SYNC.md Section 7).
    /// Combination of device ID + employee ID + timestamp where a device-native ID is unavailable.
    /// </summary>
    public string? IdempotencyKey { get; set; }

    /// <summary>Marks a duplicate event detected during sync. The canonical event is kept active.</summary>
    public bool IsDuplicate { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Employee Employee { get; set; } = null!;
    public BiometricDevice? Device { get; set; }
}
