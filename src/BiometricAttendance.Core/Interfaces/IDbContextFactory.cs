namespace BiometricAttendance.Core.Interfaces;

/// <summary>
/// Abstraction that allows Application services to create database contexts
/// without referencing EF Core or Infrastructure directly.
/// Implemented by AttendanceDbContextFactory in Infrastructure.
/// </summary>
public interface IDbContextFactory
{
    Task<IAttendanceDbContext> CreateDbContextAsync(CancellationToken ct = default);
}

/// <summary>
/// Minimal interface for the DbContext, exposing Set&lt;T&gt; for use in Application services.
/// The concrete implementation (AttendanceDbContext) lives in Infrastructure.
/// </summary>
public interface IAttendanceDbContext : IAsyncDisposable
{
    Microsoft.EntityFrameworkCore.DbSet<T> Set<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
