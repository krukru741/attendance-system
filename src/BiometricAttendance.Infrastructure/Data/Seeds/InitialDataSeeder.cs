using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BiometricAttendance.Infrastructure.Data.Seeds;

/// <summary>
/// Seeds the database with the initial required data on first run:
/// - All named permissions (from 02-AUTHENTICATION-AUTHORIZATION.md)
/// - Default roles (Administrator, HR, Supervisor, Kiosk)
/// - Role-Permission assignments
/// - Admin user (password printed to log on first creation — never hardcoded)
/// </summary>
public sealed class InitialDataSeeder
{
    private readonly AttendanceDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly ILogger<InitialDataSeeder> _logger;

    public InitialDataSeeder(AttendanceDbContext db, IPasswordHasher hasher, ILogger<InitialDataSeeder> logger)
    {
        _db = db;
        _hasher = hasher;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await _db.Database.MigrateAsync(ct);
        await SeedPermissionsAsync(ct);
        await SeedRolesAsync(ct);
        await SeedRolePermissionsAsync(ct);
        await SeedAdminUserAsync(ct);
        await SeedDefaultSettingsAsync(ct);
        await SeedShiftTemplatesAsync(ct);
    }

    // ──────────────────────────────────────────────────
    // All permissions from 02-AUTHENTICATION-AUTHORIZATION.md Section 5
    // ──────────────────────────────────────────────────
    private static readonly (string Name, string DisplayName, string Module)[] AllPermissions =
    {
        ("dashboard.view",       "View Dashboard",            "Dashboard"),
        ("employee.view",        "View Employees",            "Employee"),
        ("employee.create",      "Create Employee",           "Employee"),
        ("employee.edit",        "Edit Employee",             "Employee"),
        ("employee.archive",     "Archive Employee",          "Employee"),
        ("attendance.view",      "View Attendance",           "Attendance"),
        ("attendance.correct",   "Correct Attendance",        "Attendance"),
        ("attendance.approve",   "Approve Attendance",        "Attendance"),
        ("attendance.kiosk",     "Kiosk Attendance Capture",  "Attendance"),
        ("biometric.enroll",     "Enroll Biometric",          "Biometrics"),
        ("biometric.manage",     "Manage Biometric Devices",  "Biometrics"),
        ("schedule.view",        "View Schedules",            "Scheduling"),
        ("schedule.create",      "Create Schedule",           "Scheduling"),
        ("schedule.edit",        "Edit Schedule",             "Scheduling"),
        ("leave.view",           "View Leave",                "Leave"),
        ("leave.create",         "Create Leave Request",      "Leave"),
        ("leave.approve",        "Approve Leave",             "Leave"),
        ("overtime.view",        "View Overtime",             "Overtime"),
        ("overtime.create",      "Create Overtime Request",   "Overtime"),
        ("overtime.approve",     "Approve Overtime",          "Overtime"),
        ("reports.view",         "View Reports",              "Reports"),
        ("reports.export",       "Export Reports",            "Reports"),
        ("users.manage",         "Manage Users",              "Administration"),
        ("settings.manage",      "Manage Settings",           "Administration"),
        ("audit.view",           "View Audit Logs",           "Administration"),
        ("backup.manage",        "Manage Database Backup",    "Administration"),
    };

    private async Task SeedPermissionsAsync(CancellationToken ct)
    {
        foreach (var (name, displayName, module) in AllPermissions)
        {
            if (!await _db.Permissions.AnyAsync(p => p.Name == name, ct))
            {
                _db.Permissions.Add(new Permission
                {
                    Name = name,
                    DisplayName = displayName,
                    Module = module
                });
            }
        }
        await _db.SaveChangesAsync(ct);
    }

    private static readonly (string Name, string Description, bool IsSystem)[] DefaultRoles =
    {
        ("Administrator", "Full system access.",                                             true),
        ("HR",            "HR staff: employees, attendance, leave, overtime, reports.",      true),
        ("Supervisor",    "Team lead: team attendance, leave/overtime approval, reports.",   true),
        ("Kiosk",         "Biometric attendance capture only.",                              true),
    };

    private async Task SeedRolesAsync(CancellationToken ct)
    {
        foreach (var (name, desc, isSys) in DefaultRoles)
        {
            if (!await _db.Roles.AnyAsync(r => r.Name == name, ct))
            {
                _db.Roles.Add(new Role { Name = name, Description = desc, IsSystem = isSys });
            }
        }
        await _db.SaveChangesAsync(ct);
    }

    private async Task SeedRolePermissionsAsync(CancellationToken ct)
    {
        var roles = await _db.Roles.Include(r => r.RolePermissions).ToListAsync(ct);
        var permissions = await _db.Permissions.ToListAsync(ct);

        Permission Perm(string name) => permissions.First(p => p.Name == name);
        Role Role(string name) => roles.First(r => r.Name == name);

        void AssignIfMissing(Role role, Permission perm)
        {
            if (!role.RolePermissions.Any(rp => rp.PermissionId == perm.Id))
                _db.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = perm.Id });
        }

        var admin = Role("Administrator");
        foreach (var perm in permissions)
            AssignIfMissing(admin, perm);

        var hr = Role("HR");
        foreach (var name in new[]
        {
            "dashboard.view","employee.view","employee.create","employee.edit","employee.archive",
            "attendance.view","attendance.correct","attendance.approve",
            "biometric.enroll","biometric.manage",
            "schedule.view","schedule.create","schedule.edit",
            "leave.view","leave.create","leave.approve",
            "overtime.view","overtime.create","overtime.approve",
            "reports.view","reports.export"
        }) AssignIfMissing(hr, Perm(name));

        var supervisor = Role("Supervisor");
        foreach (var name in new[]
        {
            "dashboard.view",
            "attendance.view","attendance.correct","attendance.approve",
            "schedule.view",
            "leave.view","leave.approve",
            "overtime.view","overtime.approve",
            "reports.view"
        }) AssignIfMissing(supervisor, Perm(name));

        var kiosk = Role("Kiosk");
        AssignIfMissing(kiosk, Perm("attendance.kiosk")); // Only this permission per spec

        await _db.SaveChangesAsync(ct);
    }

    private async Task SeedAdminUserAsync(CancellationToken ct)
    {
        if (await _db.Users.AnyAsync(u => u.Username == "admin", ct))
            return;

        // Generate a secure random password — print to log only on first creation
        var tempPassword = GenerateTemporaryPassword();
        var adminRole = await _db.Roles.FirstAsync(r => r.Name == "Administrator", ct);

        var admin = new User
        {
            Username = "admin",
            DisplayName = "System Administrator",
            Email = "admin@company.local",
            PasswordHash = _hasher.Hash(tempPassword),
            MustChangePassword = true,
            IsActive = true
        };

        _db.Users.Add(admin);
        await _db.SaveChangesAsync(ct);

        _db.UserRoles.Add(new UserRole { UserId = admin.Id, RoleId = adminRole.Id });
        await _db.SaveChangesAsync(ct);

        // IMPORTANT: Log the initial password — change on first login is enforced
        _logger.LogWarning(
            "=== INITIAL ADMIN ACCOUNT CREATED ===\n" +
            "Username: admin\n" +
            "Temporary Password: {TempPassword}\n" +
            "This password will be required only once. Please change it immediately after first login.\n" +
            "=====================================",
            tempPassword);
    }

    private static string GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$";
        var bytes = new byte[16];
        System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
        return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
    }

    private async Task SeedDefaultSettingsAsync(CancellationToken ct)
    {
        var defaults = new (string Key, string Value, string Description, string Category)[]
        {
            ("company.name",                       "Your Company",   "Company name displayed in the application",      "General"),
            ("attendance.grace_period_minutes",    "10",             "Default grace period in minutes before marking Late", "Attendance"),
            ("attendance.late_threshold_minutes",  "0",             "Additional minutes before LATE is applied",         "Attendance"),
            ("attendance.min_overtime_minutes",    "30",            "Minimum minutes worked beyond shift end to count OT","Attendance"),
            ("attendance.require_ot_approval",     "true",          "Require approved overtime request for OT to count",  "Attendance"),
            ("biometric.sync_interval_minutes",    "5",             "How often to sync from biometric devices",           "Biometrics"),
        };

        foreach (var (key, value, desc, category) in defaults)
        {
            if (!await _db.SystemSettings.AnyAsync(s => s.Key == key, ct))
            {
                _db.SystemSettings.Add(new SystemSetting
                {
                    Key = key, Value = value, Description = desc, Category = category
                });
            }
        }
        await _db.SaveChangesAsync(ct);
    }

    private async Task SeedShiftTemplatesAsync(CancellationToken ct)
    {
        if (!await _db.ShiftTemplates.AnyAsync(s => s.Name == "Regular Shift", ct))
        {
            _db.ShiftTemplates.Add(new ShiftTemplate
            {
                Name = "Regular Shift",
                StartTime = new TimeOnly(8, 0),
                EndTime = new TimeOnly(17, 0),
                IsOvernight = false,
                GracePeriodMinutes = 10,
                IsActive = true
            });
            await _db.SaveChangesAsync(ct);
        }
    }
}
