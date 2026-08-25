namespace BiometricAttendance.Application.Interfaces;

public interface IPermissionService
{
    Task<IReadOnlySet<string>> GetPermissionsForUserAsync(int userId, CancellationToken ct = default);
    Task<bool> UserHasPermissionAsync(int userId, string permission, CancellationToken ct = default);
}
