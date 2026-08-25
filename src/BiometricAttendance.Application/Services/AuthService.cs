using BiometricAttendance.Application.Interfaces;
using BiometricAttendance.Core.DTOs;
using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace BiometricAttendance.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _audit;
    private readonly IPasswordHasher _hasher;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepo,
        IPermissionService permissionService,
        ICurrentUserService currentUser,
        IAuditService audit,
        IPasswordHasher hasher,
        ILogger<AuthService> logger)
    {
        _userRepo = userRepo;
        _permissionService = permissionService;
        _currentUser = currentUser;
        _audit = audit;
        _hasher = hasher;
        _logger = logger;
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Login attempt for username: {Username}", request.Username);

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return new LoginResult(false, "Username and password are required.");

        var user = await _userRepo.GetByUsernameAsync(request.Username.Trim(), ct);

        // Use a constant-time failure to resist username enumeration
        if (user is null)
        {
            _hasher.Verify("dummy", "$2a$12$dummy-hash-to-maintain-constant-time");
            await _audit.LogAsync("LOGIN_FAILED", "Auth",
                newValue: new { Username = request.Username, Reason = "UserNotFound" }, ct: ct);
            return new LoginResult(false, "Invalid username or password.");
        }

        if (!user.IsActive)
        {
            await _audit.LogAsync("LOGIN_FAILED", "Auth", entityId: user.Id.ToString(),
                newValue: new { Reason = "AccountDisabled" }, ct: ct);
            return new LoginResult(false, "Your account has been disabled. Please contact an administrator.");
        }

        // Account lockout check
        if (user.LockedUntil.HasValue && user.LockedUntil > DateTime.UtcNow)
        {
            return new LoginResult(false,
                $"Account is temporarily locked. Please try again after {user.LockedUntil.Value.ToLocalTime():HH:mm}.");
        }

        if (!_hasher.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockedUntil = DateTime.UtcNow.AddMinutes(15);
                _logger.LogWarning("Account locked after {Attempts} failed attempts: {Username}", 
                    user.FailedLoginAttempts, user.Username);
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _userRepo.UpdateAsync(user, ct);
            await _userRepo.SaveChangesAsync(ct);
            await _audit.LogAsync("LOGIN_FAILED", "Auth", entityId: user.Id.ToString(),
                newValue: new { Reason = "WrongPassword", Attempts = user.FailedLoginAttempts }, ct: ct);
            return new LoginResult(false, "Invalid username or password.");
        }

        // Successful login
        user.FailedLoginAttempts = 0;
        user.LockedUntil = null;
        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepo.UpdateAsync(user, ct);
        await _userRepo.SaveChangesAsync(ct);

        var permissions = await _permissionService.GetPermissionsForUserAsync(user.Id, ct);
        _currentUser.SetUser(user, permissions);

        await _audit.LogAsync("LOGIN", "Auth", entityId: user.Id.ToString(), ct: ct);
        _logger.LogInformation("Login successful: {Username} ({UserId})", user.Username, user.Id);

        return new LoginResult(true, UserId: user.Id, DisplayName: user.DisplayName, Permissions: permissions);
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        var userId = _currentUser.CurrentUserId;
        _currentUser.ClearUser();
        if (userId.HasValue)
            await _audit.LogAsync("LOGOUT", "Auth", entityId: userId.ToString(), ct: ct);
        _logger.LogInformation("User {UserId} logged out.", userId);
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(userId, ct)
            ?? throw new Core.Exceptions.NotFoundException(nameof(User), userId);

        if (!_hasher.Verify(currentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = _hasher.Hash(newPassword);
        user.MustChangePassword = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepo.UpdateAsync(user, ct);
        await _userRepo.SaveChangesAsync(ct);
        await _audit.LogAsync("PASSWORD_CHANGED", "Auth", entityId: userId.ToString(), ct: ct);
        return true;
    }
}
