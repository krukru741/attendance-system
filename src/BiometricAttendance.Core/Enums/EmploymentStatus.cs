namespace BiometricAttendance.Core.Enums;

/// <summary>
/// Drives the Employee lifecycle. This is the single source of truth for employee
/// active/inactive/archived state — no separate IsActive/IsArchived booleans are used
/// on the Employee entity (Decision 1).
/// </summary>
public enum EmploymentStatus
{
    /// <summary>Currently employed and active.</summary>
    Active,

    /// <summary>Employed but temporarily inactive (leave of absence, suspension, etc.).</summary>
    Inactive,

    /// <summary>
    /// Soft-deleted. The record is retained for historical attendance data but
    /// excluded from all active-employee queries.
    /// </summary>
    Archived
}
