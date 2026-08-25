namespace BiometricAttendance.Core.Entities;

public class LeaveBalance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public int Year { get; set; }
    public decimal Entitled { get; set; } = 0;
    public decimal Used { get; set; } = 0;
    public decimal Remaining => Entitled - Used;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Employee Employee { get; set; } = null!;
    public LeaveType LeaveType { get; set; } = null!;
}
