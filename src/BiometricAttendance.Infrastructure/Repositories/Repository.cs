using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Interfaces;
using BiometricAttendance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiometricAttendance.Infrastructure.Repositories;

public abstract class Repository<T> : IRepository<T> where T : class
{
    protected readonly AttendanceDbContext _db;

    protected Repository(AttendanceDbContext db) => _db = db;

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Set<T>().FindAsync(new object[] { id }, ct);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await _db.Set<T>().ToListAsync(ct);

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
        => await _db.Set<T>().AddAsync(entity, ct);

    public virtual Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
