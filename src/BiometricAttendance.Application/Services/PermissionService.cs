using BiometricAttendance.Application.Interfaces;
using BiometricAttendance.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiometricAttendance.Application.Services;

public sealed class PermissionService : IPermissionService
{
    private readonly IDbContextFactory _dbContextFactory;

    public PermissionService(IDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IReadOnlySet<string>> GetPermissionsForUserAsync(int userId, CancellationToken ct = default)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(ct);

        var permissions = await db.Set<Core.Entities.UserRole>()
            .Where(ur => ur.UserId == userId && ur.Role.IsActive)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync(ct);

        return new HashSet<string>(permissions, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<bool> UserHasPermissionAsync(int userId, string permission, CancellationToken ct = default)
    {
        var permissions = await GetPermissionsForUserAsync(userId, ct);
        return permissions.Contains(permission);
    }
}
