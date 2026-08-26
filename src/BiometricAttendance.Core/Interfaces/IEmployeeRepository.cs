using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Enums;

namespace BiometricAttendance.Core.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByCodeAsync(string employeeCode, CancellationToken ct = default);
    Task<bool> EmployeeCodeExistsAsync(string employeeCode, CancellationToken ct = default);
    Task<IReadOnlyList<Employee>> GetFilteredAsync(string? searchTerm, int? departmentId, EmploymentStatus? status, CancellationToken ct = default);
}
