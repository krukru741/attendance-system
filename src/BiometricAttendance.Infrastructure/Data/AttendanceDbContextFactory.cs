using BiometricAttendance.Core.Interfaces;

namespace BiometricAttendance.Infrastructure.Data;

public sealed class AttendanceDbContextFactory : BiometricAttendance.Core.Interfaces.IDbContextFactory
{
    private readonly Microsoft.EntityFrameworkCore.IDbContextFactory<AttendanceDbContext> _factory;

    public AttendanceDbContextFactory(
        Microsoft.EntityFrameworkCore.IDbContextFactory<AttendanceDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IAttendanceDbContext> CreateDbContextAsync(CancellationToken ct = default)
        => await _factory.CreateDbContextAsync(ct);
}
