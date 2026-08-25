using BiometricAttendance.Core.Entities;

namespace BiometricAttendance.Core.Interfaces;

/// <summary>
/// Provides the currently authenticated user's identity and permissions to the application layer.
/// Populated on login; cleared on logout.
/// </summary>
public interface ICurrentUserService
{
    User? CurrentUser { get; }
    int? CurrentUserId { get; }
    bool IsAuthenticated { get; }
    IReadOnlySet<string> Permissions { get; }

    bool HasPermission(string permission);

    void SetUser(User user, IEnumerable<string> permissions);
    void ClearUser();
}
