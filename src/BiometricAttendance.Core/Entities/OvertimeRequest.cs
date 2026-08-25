using BiometricAttendance.Core.Enums;

namespace BiometricAttendance.Core.Entities;

public class OvertimeRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int RequestedMinutes { get; set; }
    public int? ApprovedMinutes { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? AttachmentPath { get; set; }
    public OvertimeRequestStatus Status { get; set; } = OvertimeRequestStatus.Pending;

    public int? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ReviewComment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Employee Employee { get; set; } = null!;
    public User? ApprovedBy { get; set; }
}
