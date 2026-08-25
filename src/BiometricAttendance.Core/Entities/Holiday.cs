using BiometricAttendance.Core.Enums;

namespace BiometricAttendance.Core.Entities;

/// <summary>
/// A company or statutory holiday. When a date is marked as a holiday,
/// the Attendance Engine assigns HOLIDAY status rather than ABSENT.
/// </summary>
public class Holiday
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public HolidayType Type { get; set; }
    public bool IsPaid { get; set; } = true;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
