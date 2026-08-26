using BiometricAttendance.Application.Interfaces;
using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Enums;
using BiometricAttendance.Core.Interfaces;
using System.Transactions;

namespace BiometricAttendance.Application.Services;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUser;

    public EmployeeService(
        IEmployeeRepository repository,
        IAuditService auditService,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _auditService = auditService;
        _currentUser = currentUser;
    }

    public async Task<Employee> CreateEmployeeAsync(Employee employee, CancellationToken ct = default)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (await _repository.EmployeeCodeExistsAsync(employee.EmployeeCode, ct))
        {
            throw new InvalidOperationException($"Employee with code {employee.EmployeeCode} already exists.");
        }

        employee.CreatedAt = DateTime.UtcNow;
        employee.UpdatedAt = DateTime.UtcNow;
        employee.EmploymentStatus = EmploymentStatus.Active; // Explicitly ensure status per spec

        await _repository.AddAsync(employee, ct);
        await _repository.SaveChangesAsync(ct);

        await _auditService.LogAsync(
            "Employee creation",
            $"Created employee {employee.EmployeeCode} ({employee.FullName})",
            _currentUser.UserId,
            ct);

        scope.Complete();
        return employee;
    }

    public async Task<Employee> UpdateEmployeeAsync(Employee employee, CancellationToken ct = default)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var existing = await _repository.GetByIdAsync(employee.Id, ct);
        if (existing == null)
            throw new ArgumentException("Employee not found.", nameof(employee.Id));

        if (existing.EmployeeCode != employee.EmployeeCode)
        {
            if (await _repository.EmployeeCodeExistsAsync(employee.EmployeeCode, ct))
            {
                throw new InvalidOperationException($"Employee with code {employee.EmployeeCode} already exists.");
            }
        }

        // Apply changes
        existing.EmployeeCode = employee.EmployeeCode;
        existing.FirstName = employee.FirstName;
        existing.MiddleName = employee.MiddleName;
        existing.LastName = employee.LastName;
        existing.Suffix = employee.Suffix;
        existing.BirthDate = employee.BirthDate;
        existing.Gender = employee.Gender;
        existing.Phone = employee.Phone;
        existing.Email = employee.Email;
        existing.Address = employee.Address;
        existing.DepartmentId = employee.DepartmentId;
        existing.PositionId = employee.PositionId;
        existing.EmploymentType = employee.EmploymentType;
        existing.DateHired = employee.DateHired;
        existing.SupervisorId = employee.SupervisorId;
        existing.DefaultShiftTemplateId = employee.DefaultShiftTemplateId;
        existing.GracePeriodMinutes = employee.GracePeriodMinutes;
        existing.IsOvertimeEligible = employee.IsOvertimeEligible;
        existing.RestDays = employee.RestDays;

        existing.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(existing, ct);
        await _repository.SaveChangesAsync(ct);

        await _auditService.LogAsync(
            "Employee modification",
            $"Updated employee {existing.EmployeeCode}",
            _currentUser.UserId,
            ct);

        scope.Complete();
        return existing;
    }

    public async Task ArchiveEmployeeAsync(int employeeId, CancellationToken ct = default)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var existing = await _repository.GetByIdAsync(employeeId, ct);
        if (existing == null)
            throw new ArgumentException("Employee not found.", nameof(employeeId));

        existing.EmploymentStatus = EmploymentStatus.Archived;
        existing.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(existing, ct);
        await _repository.SaveChangesAsync(ct);

        await _auditService.LogAsync(
            "Employee archived",
            $"Archived employee {existing.EmployeeCode}",
            _currentUser.UserId,
            ct);

        scope.Complete();
    }
}
