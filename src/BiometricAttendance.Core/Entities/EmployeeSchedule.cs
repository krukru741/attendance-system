namespace BiometricAttendance.Core.Entities;

/// <summary>
/// A concrete work schedule assignment for one employee on one date.
/// Uses DateTime for start/end to correctly handle overnight shifts crossing midnight
/// (per 07-SCHEDULING.md Section 8 and 06-ATTENDANCE.md Section 9).
/// </summary>
public class EmployeeSchedule
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int? ShiftTemplateId { get; set; }
    public DateOnly Date { get; set; }

    /// <summary>Absolute start DateTime — resolved from Date + ShiftTemplate.StartTime.</summary>
    public DateTime Start { get; set; }

    /// <summary>
    /// Absolute end DateTime — may be the following day for overnight shifts.
    /// Never assume End.Date == Start.Date.
    /// </summary>
    public DateTime End { get; set; }

    public bool IsRestDay { get; set; } = false;
    public string? ScheduleStatus { get; set; } // e.g. Active, Cancelled, Modified
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Employee Employee { get; set; } = null!;
    public ShiftTemplate? ShiftTemplate { get; set; }
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
}
