using BiometricAttendance.Core.Enums;

namespace BiometricAttendance.Core.Entities;

/// <summary>
/// Stores an encrypted biometric template for one finger of one employee.
/// Raw fingerprint images are NOT stored per 12-DATABASE-SCHEMA.md Section 6.
/// </summary>
public class EmployeeBiometric
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public FingerType FingerType { get; set; }

    /// <summary>Encrypted biometric template bytes (vendor SDK format).</summary>
    public byte[] Template { get; set; } = Array.Empty<byte>();

    /// <summary>Identifies which vendor SDK format the template was generated with.</summary>
    public string TemplateFormat { get; set; } = string.Empty;

    public int? DeviceId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Employee Employee { get; set; } = null!;
    public BiometricDevice? Device { get; set; }
}
