namespace BiometricAttendance.Core.Enums;

/// <summary>
/// Reflects the last observed operational state of a BiometricDevice.
/// "Disabled" is NOT a value here — disabling is expressed via BiometricDevice.IsActive = false
/// (soft-delete flag). This keeps the two fields non-overlapping (Decision 2).
/// </summary>
public enum BiometricDeviceStatus
{
    /// <summary>Device is reachable and the last connection succeeded.</summary>
    Online,

    /// <summary>Device is not reachable.</summary>
    Offline,

    /// <summary>A sync operation is currently in progress.</summary>
    Syncing,

    /// <summary>The last operation resulted in an error.</summary>
    Error
}
