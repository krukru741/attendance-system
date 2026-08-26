using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Enums;
using BiometricAttendance.Core.Interfaces;
using BiometricAttendance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiometricAttendance.Infrastructure.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly AttendanceDbContext _db;

    public EmployeeRepository(AttendanceDbContext db)
    {
        _db = db;
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.DefaultShiftTemplate)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Employee entity, CancellationToken ct = default)
    {
        _db.Employees.Add(entity);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(Employee entity, CancellationToken ct = default)
    {
        _db.Employees.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Employee entity, CancellationToken ct = default)
    {
        _db.Employees.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _db.SaveChangesAsync(ct);
    }

    public async Task<Employee?> GetByCodeAsync(string employeeCode, CancellationToken ct = default)
    {
        return await _db.Employees
            .FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode, ct);
    }

    public async Task<bool> EmployeeCodeExistsAsync(string employeeCode, CancellationToken ct = default)
    {
        return await _db.Employees.AnyAsync(e => e.EmployeeCode == employeeCode, ct);
    }

    public async Task<IReadOnlyList<Employee>> GetFilteredAsync(string? searchTerm, int? departmentId, EmploymentStatus? status, CancellationToken ct = default)
    {
        var query = _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(e => e.EmploymentStatus == status.Value);
        }

        if (departmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == departmentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(e =>
                e.EmployeeCode.ToLower().Contains(searchTerm) ||
                e.FirstName.ToLower().Contains(searchTerm) ||
                e.LastName.ToLower().Contains(searchTerm));
        }

        return await query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToListAsync(ct);
    }
}
