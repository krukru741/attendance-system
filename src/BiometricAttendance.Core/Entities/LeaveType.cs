namespace BiometricAttendance.Core.Entities;

public class LeaveType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g. Vacation Leave, Sick Leave
    public string? Description { get; set; }
    public int DefaultDaysPerYear { get; set; } = 0;
    public bool RequiresApproval { get; set; } = true;
    public bool RequiresAttachment { get; set; } = false;
    public bool IsPaid { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
}
