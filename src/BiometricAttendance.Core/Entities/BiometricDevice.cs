using BiometricAttendance.Core.Enums;

namespace BiometricAttendance.Core.Entities;

/// <summary>
/// Biometric scanner/reader device. 
/// Status = operational state (Decision 2).
/// IsActive = soft-delete flag (Decision 2).
/// The two fields never overlap.
/// </summary>
public class BiometricDevice
{
    public int Id { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string? IPAddress { get; set; }
    public int? Port { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? FirmwareVersion { get; set; }

    /// <summary>
    /// Last observed operational state. Updated after every connection attempt or sync.
    /// Does NOT encode "disabled" — that is IsActive = false.
    /// </summary>
    public BiometricDeviceStatus Status { get; set; } = BiometricDeviceStatus.Offline;

    /// <summary>
    /// Soft-delete flag. False means this device has been removed from the system.
    /// Excluded from all active-device queries when false.
    /// </summary>
    public bool IsActive { get; set; } = true;

    public DateTime? LastSyncAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<BiometricLog> Logs { get; set; } = new List<BiometricLog>();
    public ICollection<AttendanceEvent> AttendanceEvents { get; set; } = new List<AttendanceEvent>();
}
