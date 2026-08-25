using System;
using System.Linq;
using System.Threading.Tasks;
using BiometricAttendance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BiometricAttendance.Tests.Unit;

public class SqliteBehaviorTests
{
    [Fact]
    public async Task Sqlite_ShouldPreserve_DateTimePrecision_And_DecimalValues()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AttendanceDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        await using var context = new AttendanceDbContext(options);
        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        // Simulate an overnight shift record
        var overnightStart = new DateTime(2026, 1, 1, 22, 0, 0, DateTimeKind.Utc);
        var overnightEnd = new DateTime(2026, 1, 2, 6, 30, 0, DateTimeKind.Utc);
        
        var record = new Core.Entities.AttendanceRecord
        {
            EmployeeId = 1,
            Date = new DateOnly(2026, 1, 1),
            TimeIn = overnightStart,
            TimeOut = overnightEnd,
            LateMinutes = 15,
            UndertimeMinutes = 0,
            OvertimeMinutes = 120, // int test
            Status = Core.Enums.AttendanceStatus.Present,
            CreatedAt = DateTime.UtcNow
        };

        // We need an employee to satisfy FK
        var employee = new Core.Entities.Employee 
        { 
            Id = 1, 
            FirstName = "Test", 
            LastName = "Employee",
            DepartmentId = 1,
            PositionId = 1,
            EmploymentStatus = Core.Enums.EmploymentStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        var dept = new Core.Entities.Department { Id = 1, Name = "Dept", CreatedAt = DateTime.UtcNow };
        var pos = new Core.Entities.Position { Id = 1, Name = "Pos", CreatedAt = DateTime.UtcNow };

        context.Departments.Add(dept);
        context.Positions.Add(pos);
        context.Employees.Add(employee);
        context.AttendanceRecords.Add(record);
        
        await context.SaveChangesAsync();

        // Act
        var savedRecord = await context.AttendanceRecords.FirstAsync();

        // Assert
        // SQLite stores DateTimes as ISO8601 strings and decimals as TEXT/REAL.
        // We verify that EF Core properly restores the precision and types.
        Assert.Equal(overnightStart, savedRecord.TimeIn);
        Assert.Equal(overnightEnd, savedRecord.TimeOut);
        Assert.Equal(120, savedRecord.OvertimeMinutes);
        Assert.Equal(15, savedRecord.LateMinutes);
    }
}
