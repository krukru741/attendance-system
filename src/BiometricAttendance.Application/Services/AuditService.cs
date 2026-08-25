using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BiometricAttendance.Application.Services;

public sealed class AuditService : IAuditService
{
    private readonly IDbContextFactory _dbContextFactory;
    private readonly ICurrentUserService _currentUser;

    public AuditService(IDbContextFactory dbContextFactory, ICurrentUserService currentUser)
    {
        _dbContextFactory = dbContextFactory;
        _currentUser = currentUser;
    }

    public async Task LogAsync(
        string action,
        string module,
        string? entityName = null,
        string? entityId = null,
        object? oldValue = null,
        object? newValue = null,
        CancellationToken ct = default)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(ct);

        var log = new AuditLog
        {
            UserId = _currentUser.CurrentUserId,
            Action = action,
            Module = module,
            EntityName = entityName,
            EntityId = entityId,
            OldValue = oldValue is null ? null : JsonSerializer.Serialize(oldValue),
            NewValue = newValue is null ? null : JsonSerializer.Serialize(newValue),
            Timestamp = DateTime.UtcNow,
            MachineName = Environment.MachineName
        };

        db.Set<AuditLog>().Add(log);
        await db.SaveChangesAsync(ct);
    }
}
