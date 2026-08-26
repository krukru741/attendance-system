using BiometricAttendance.Application.Services;
using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Enums;
using BiometricAttendance.Core.Interfaces;
using Moq;
using Xunit;

namespace BiometricAttendance.Tests.Unit;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _repoMock;
    private readonly Mock<IAuditService> _auditMock;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        _repoMock = new Mock<IEmployeeRepository>();
        _auditMock = new Mock<IAuditService>();

        _service = new EmployeeService(_repoMock.Object, _auditMock.Object);
    }

    [Fact]
    public async Task CreateEmployee_ThrowsIfCodeExists()
    {
        // Arrange
        var employee = new Employee { EmployeeCode = "EMP-001" };
        _repoMock.Setup(r => r.EmployeeCodeExistsAsync("EMP-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.CreateEmployeeAsync(employee));
        
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Never);
        _auditMock.Verify(a => a.LogAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<object>(), It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateEmployee_SetsActiveStatusAndLogsAudit()
    {
        // Arrange
        var employee = new Employee { EmployeeCode = "EMP-002", FirstName = "John", LastName = "Doe" };
        _repoMock.Setup(r => r.EmployeeCodeExistsAsync("EMP-002", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.CreateEmployeeAsync(employee);

        // Assert
        Assert.Equal(EmploymentStatus.Active, result.EmploymentStatus);
        
        _repoMock.Verify(r => r.AddAsync(employee, It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
        _auditMock.Verify(a => a.LogAsync(
            "Create", "Employee", "Employee", employee.EmployeeCode,
            null, employee, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ArchiveEmployee_ChangesStatusToArchivedAndLogsAudit()
    {
        // Arrange
        var employee = new Employee { Id = 5, EmployeeCode = "EMP-005", EmploymentStatus = EmploymentStatus.Active };
        _repoMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        // Act
        await _service.ArchiveEmployeeAsync(5);

        // Assert
        Assert.Equal(EmploymentStatus.Archived, employee.EmploymentStatus);
        
        _repoMock.Verify(r => r.UpdateAsync(employee, It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
        _auditMock.Verify(a => a.LogAsync(
            "Archive", "Employee", "Employee", employee.EmployeeCode,
            null, It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
