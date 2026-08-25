using BiometricAttendance.Core.Enums;

namespace BiometricAttendance.Core.Entities;

/// <summary>
/// Represents an approved or pending correction applied on top of an AttendanceRecord.
/// The original AttendanceEvent is NEVER modified — corrections are layered on top.
/// Traceability chain: AttendanceEvent → AttendanceRecord → AttendanceCorrection → AuditLog
/// </summary>
public class AttendanceCorrection
{
    public int Id { get; set; }
    public int AttendanceRecordId { get; set; }
    public int EmployeeId { get; set; }
    public CorrectionType CorrectionType { get; set; }

    /// <summary>JSON or string representation of the value before correction.</summary>
    public string? OriginalValue { get; set; }

    /// <summary>JSON or string representation of the requested corrected value.</summary>
    public string? RequestedValue { get; set; }

    public string Reason { get; set; } = string.Empty;
    public string? AttachmentPath { get; set; }
    public CorrectionStatus Status { get; set; } = CorrectionStatus.Pending;

    public int RequestedByUserId { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public int? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewComment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AttendanceRecord AttendanceRecord { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public User RequestedBy { get; set; } = null!;
    public User? ReviewedBy { get; set; }
}
