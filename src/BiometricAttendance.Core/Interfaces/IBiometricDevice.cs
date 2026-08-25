namespace BiometricAttendance.Core.Interfaces;

/// <summary>
/// Abstraction over one biometric scanner device.
/// All biometric hardware must be accessed exclusively through this interface.
/// The Attendance Engine and Application layer must NEVER reference vendor SDKs directly.
/// Phase 4: MockBiometricDevice implements this for testing without hardware.
/// Phase 5: Vendor-specific adapters (e.g. ZKTecoAdapter) implement this.
/// </summary>
public interface IBiometricDevice
{
    Task<bool> ConnectAsync(CancellationToken ct = default);
    Task DisconnectAsync(CancellationToken ct = default);
    Task<BiometricTemplate> EnrollAsync(CancellationToken ct = default);
    Task<BiometricMatchResult> VerifyAsync(CancellationToken ct = default);
    Task<IReadOnlyList<BiometricAttendanceEvent>> GetAttendanceLogsAsync(CancellationToken ct = default);
    Task<bool> PushUserAsync(int employeeId, byte[] template, CancellationToken ct = default);
    bool IsConnected { get; }
}

// --- Supporting value objects (device-agnostic) ---

public sealed record BiometricTemplate(
    byte[] Data,
    string Format,
    int Quality,
    int FingerIndex);

public sealed record BiometricMatchResult(
    bool IsMatch,
    int EmployeeId,
    float Score);

public sealed record BiometricAttendanceEvent(
    string DeviceEventId,      // Device-native event ID for deduplication
    int EmployeeId,
    DateTime Timestamp,
    string EventTypeRaw        // Raw event type string from device; mapped to AttendanceEventType
);
