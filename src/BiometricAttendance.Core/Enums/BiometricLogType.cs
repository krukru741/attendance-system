namespace BiometricAttendance.Core.Enums;

/// <summary>
/// Classifies a BiometricLog entry per 05-BIOMETRICS.md Section 6.2.
/// </summary>
public enum BiometricLogType
{
    ConnectionTest,
    Sync,
    PushUser,
    PullLogs,
    Error
}
