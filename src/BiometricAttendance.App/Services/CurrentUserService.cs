using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Interfaces;

namespace BiometricAttendance.App.Services;

/// <summary>
/// In-memory session state for the currently authenticated user.
/// Lives as a singleton for the application lifetime.
/// Populated on login, cleared on logout.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private User? _user;
    private IReadOnlySet<string> _permissions = new HashSet<string>();

    public User? CurrentUser => _user;
    public int? CurrentUserId => _user?.Id;
    public bool IsAuthenticated => _user is not null;
    public IReadOnlySet<string> Permissions => _permissions;

    public bool HasPermission(string permission)
        => _permissions.Contains(permission);

    public void SetUser(User user, IEnumerable<string> permissions)
    {
        _user = user;
        _permissions = new HashSet<string>(permissions, StringComparer.OrdinalIgnoreCase);
    }

    public void ClearUser()
    {
        _user = null;
        _permissions = new HashSet<string>();
    }
}
