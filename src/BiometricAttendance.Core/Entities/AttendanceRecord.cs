using BiometricAttendance.Core.Enums;

namespace BiometricAttendance.Core.Entities;

/// <summary>
/// Processed attendance record computed by the Attendance Engine.
/// This is what reports are built from — not raw events directly.
/// </summary>
public class AttendanceRecord
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int? ScheduleId { get; set; }
    public DateOnly Date { get; set; }
    public DateTime? TimeIn { get; set; }
    public DateTime? BreakOut { get; set; }
    public DateTime? BreakIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public int WorkedMinutes { get; set; } = 0;
    public int LateMinutes { get; set; } = 0;
    public int UndertimeMinutes { get; set; } = 0;
    public int OvertimeMinutes { get; set; } = 0;
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Absent;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Employee Employee { get; set; } = null!;
    public EmployeeSchedule? Schedule { get; set; }
    public ICollection<AttendanceCorrection> Corrections { get; set; } = new List<AttendanceCorrection>();
}
