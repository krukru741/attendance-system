namespace BiometricAttendance.Core.DTOs;

public sealed record LoginRequest(string Username, string Password);

public sealed record LoginResult(
    bool Success,
    string? ErrorMessage = null,
    int? UserId = null,
    string? DisplayName = null,
    IReadOnlySet<string>? Permissions = null
);
