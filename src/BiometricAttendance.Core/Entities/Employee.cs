using BiometricAttendance.Core.Enums;

namespace BiometricAttendance.Core.Entities;

/// <summary>
/// Core employee entity. Lifecycle is governed entirely by <see cref="EmploymentStatus"/>
/// (Decision 1 — no IsActive/IsArchived booleans on this entity).
/// </summary>
public class Employee
{
    public int Id { get; set; }

    // --- Identity ---
    public string EmployeeCode { get; set; } = string.Empty; // e.g. EMP-001
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? Suffix { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? PhotoPath { get; set; }

    // --- Employment ---
    public int? DepartmentId { get; set; }
    public int? PositionId { get; set; }
    public string? EmploymentType { get; set; }    // Regular, Probationary, Contractual, etc.
    public DateTime? DateHired { get; set; }

    /// <summary>
    /// Drives the employee lifecycle. Active queries must filter on this value.
    /// Never use boolean flags for lifecycle — see Decision 1.
    /// </summary>
    public EmploymentStatus EmploymentStatus { get; set; } = EmploymentStatus.Active;

    public int? SupervisorId { get; set; }

    // --- Attendance configuration ---
    public int? DefaultShiftTemplateId { get; set; }
    public int GracePeriodMinutes { get; set; } = 0; // 0 = inherit from system settings
    public bool IsOvertimeEligible { get; set; } = false;
    public string? RestDays { get; set; } // Comma-separated day numbers: "0,6" = Sunday, Saturday

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Department? Department { get; set; }
    public Position? Position { get; set; }
    public Employee? Supervisor { get; set; }
    public ShiftTemplate? DefaultShiftTemplate { get; set; }
    public ICollection<EmployeeBiometric> Biometrics { get; set; } = new List<EmployeeBiometric>();
    public ICollection<EmployeeSchedule> Schedules { get; set; } = new List<EmployeeSchedule>();
    public ICollection<AttendanceEvent> AttendanceEvents { get; set; } = new List<AttendanceEvent>();
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<OvertimeRequest> OvertimeRequests { get; set; } = new List<OvertimeRequest>();

    // --- Computed ---
    public string FullName => string.Join(" ",
        new[] { FirstName, MiddleName, LastName, Suffix }
        .Where(s => !string.IsNullOrWhiteSpace(s)));
}
