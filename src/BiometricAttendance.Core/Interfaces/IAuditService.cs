namespace BiometricAttendance.Core.Interfaces;

/// <summary>
/// Records security-sensitive actions to the AuditLog table.
/// Called at the application service layer after every auditable event.
/// </summary>
public interface IAuditService
{
    Task LogAsync(
        string action,
        string module,
        string? entityName = null,
        string? entityId = null,
        object? oldValue = null,
        object? newValue = null,
        CancellationToken ct = default);
}
