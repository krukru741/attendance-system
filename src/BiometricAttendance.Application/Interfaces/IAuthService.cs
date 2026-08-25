using BiometricAttendance.Core.DTOs;

namespace BiometricAttendance.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task LogoutAsync(CancellationToken ct = default);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken ct = default);
}
