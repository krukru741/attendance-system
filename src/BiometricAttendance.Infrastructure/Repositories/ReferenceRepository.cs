using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Interfaces;
using BiometricAttendance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiometricAttendance.Infrastructure.Repositories;

public sealed class ReferenceRepository : IReferenceRepository
{
    private readonly AttendanceDbContext _db;

    public ReferenceRepository(AttendanceDbContext db)
    {
        _db = db;
    }

    public Task<List<Department>> GetDepartmentsAsync(CancellationToken ct = default)
    {
        return _db.Departments.OrderBy(d => d.Name).ToListAsync(ct);
    }

    public Task<List<Position>> GetPositionsAsync(CancellationToken ct = default)
    {
        return _db.Positions.OrderBy(p => p.Name).ToListAsync(ct);
    }

    public Task<List<ShiftTemplate>> GetShiftTemplatesAsync(CancellationToken ct = default)
    {
        return _db.ShiftTemplates.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync(ct);
    }
}
