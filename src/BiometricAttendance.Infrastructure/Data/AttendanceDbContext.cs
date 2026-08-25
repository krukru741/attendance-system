using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BiometricAttendance.Infrastructure.Data;

public sealed class AttendanceDbContext : DbContext, IAttendanceDbContext
{
    public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options) : base(options) { }

    // Identity
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // Workforce
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();

    // Biometrics
    public DbSet<BiometricDevice> BiometricDevices => Set<BiometricDevice>();
    public DbSet<EmployeeBiometric> EmployeeBiometrics => Set<EmployeeBiometric>();
    public DbSet<BiometricLog> BiometricLogs => Set<BiometricLog>();

    // Attendance
    public DbSet<AttendanceEvent> AttendanceEvents => Set<AttendanceEvent>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<AttendanceCorrection> AttendanceCorrections => Set<AttendanceCorrection>();

    // Scheduling
    public DbSet<ShiftTemplate> ShiftTemplates => Set<ShiftTemplate>();
    public DbSet<EmployeeSchedule> EmployeeSchedules => Set<EmployeeSchedule>();
    public DbSet<Holiday> Holidays => Set<Holiday>();

    // Leave
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();

    // Overtime
    public DbSet<OvertimeRequest> OvertimeRequests => Set<OvertimeRequest>();

    // System
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AttendanceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
