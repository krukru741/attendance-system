using BiometricAttendance.Core.Enums;

namespace BiometricAttendance.Core.Entities;

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal NumberOfDays { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? AttachmentPath { get; set; }
    public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Draft;

    public int SubmittedByUserId { get; set; }
    public DateTime? SubmittedAt { get; set; }

    public int? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewComment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Employee Employee { get; set; } = null!;
    public LeaveType LeaveType { get; set; } = null!;
    public User SubmittedBy { get; set; } = null!;
    public User? ReviewedBy { get; set; }
}
