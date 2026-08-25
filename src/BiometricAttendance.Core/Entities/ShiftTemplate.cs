namespace BiometricAttendance.Core.Entities;

/// <summary>
/// Reusable shift definition. Provides defaults that EmployeeSchedule instances can override.
/// Supports overnight shifts via Start/EndTime as TimeOnly — the actual date context is
/// resolved at the EmployeeSchedule level.
/// </summary>
public class ShiftTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public TimeOnly? BreakStart { get; set; }
    public TimeOnly? BreakEnd { get; set; }
    public int GracePeriodMinutes { get; set; } = 10;
    public int LateThresholdMinutes { get; set; } = 0;
    public int EarlyOutThresholdMinutes { get; set; } = 0;
    public int OvertimeThresholdMinutes { get; set; } = 0;

    /// <summary>True when the shift crosses midnight (e.g. 22:00–07:00).</summary>
    public bool IsOvernight { get; set; } = false;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<EmployeeSchedule> Schedules { get; set; } = new List<EmployeeSchedule>();
}
