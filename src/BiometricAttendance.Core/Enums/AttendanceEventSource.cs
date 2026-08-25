namespace BiometricAttendance.Core.Enums;

/// <summary>
/// Tracks how an AttendanceEvent was originated.
/// </summary>
public enum AttendanceEventSource
{
    /// <summary>Captured by a physical biometric scanner.</summary>
    BiometricDevice,

    /// <summary>Manually entered by HR or an administrator.</summary>
    Manual,

    /// <summary>Imported from an offline/local sync queue.</summary>
    OfflineSync,

    /// <summary>Injected by the mock adapter during testing (Phase 4).</summary>
    MockDevice
}
