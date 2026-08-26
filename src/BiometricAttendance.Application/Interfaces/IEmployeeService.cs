using BiometricAttendance.Core.Entities;

namespace BiometricAttendance.Application.Interfaces;

public interface IEmployeeService
{
    Task<Employee> CreateEmployeeAsync(Employee employee, CancellationToken ct = default);
    Task<Employee> UpdateEmployeeAsync(Employee employee, CancellationToken ct = default);
    Task ArchiveEmployeeAsync(int employeeId, CancellationToken ct = default);
}
