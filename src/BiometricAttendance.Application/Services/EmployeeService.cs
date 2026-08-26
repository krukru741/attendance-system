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
    public EmployeeService(
        IEmployeeRepository repository,
        IAuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
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
            action: "Create",
            module: "Employee",
            entityName: "Employee",
            entityId: employee.EmployeeCode,
            newValue: employee,
            ct: ct);

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
            action: "Update",
            module: "Employee",
            entityName: "Employee",
            entityId: existing.EmployeeCode,
            newValue: existing,
            ct: ct);

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
            action: "Archive",
            module: "Employee",
            entityName: "Employee",
            entityId: existing.EmployeeCode,
            newValue: new { EmploymentStatus = EmploymentStatus.Archived },
            ct: ct);

        scope.Complete();
    }
}
