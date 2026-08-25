using BiometricAttendance.Application.Interfaces;
using BiometricAttendance.Application.Services;
using BiometricAttendance.Core.DTOs;
using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Interfaces;
using Moq;

namespace BiometricAttendance.Tests.Unit;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IPermissionService> _permissionMock = new();
    private readonly Mock<IPasswordHasher> _hasherMock = new();
    private readonly Mock<IAuditService> _auditMock = new();
    private readonly Mock<ICurrentUserService> _currentUserMock = new();
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<AuthService>> _loggerMock = new();
    
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _authService = new AuthService(
            _userRepoMock.Object,
            _permissionMock.Object,
            _currentUserMock.Object,
            _auditMock.Object,
            _hasherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccessAndSetsUser()
    {
        // Arrange
        var user = new User 
        { 
            Username = "admin", 
            PasswordHash = "hashed_pwd", 
            DisplayName = "Admin", 
            Email = "admin@local.test",
            Id = 1,
            IsActive = true
        };
        user.UserRoles.Add(new UserRole { Role = new Role { Name = "Administrator" } });

        _userRepoMock.Setup(x => x.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _hasherMock.Setup(x => x.Verify("valid_pwd", "hashed_pwd"))
            .Returns(true);

        // Act
        var result = await _authService.LoginAsync(new LoginRequest("admin", "valid_pwd"));

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.ErrorMessage);

        _currentUserMock.Verify(x => x.SetUser(user, It.IsAny<IEnumerable<string>>()), Times.Once);
        _auditMock.Verify(x => x.LogAsync("LOGIN", "Auth", null, It.IsAny<string>(), null, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsFailure()
    {
        // Arrange
        var user = new User 
        { 
            Username = "admin", 
            PasswordHash = "hashed_pwd", 
            DisplayName = "Admin", 
            Email = "admin@local.test",
            Id = 1,
            IsActive = true
        };

        _userRepoMock.Setup(x => x.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _hasherMock.Setup(x => x.Verify("wrong_pwd", "hashed_pwd"))
            .Returns(false);

        // Act
        var result = await _authService.LoginAsync(new LoginRequest("admin", "wrong_pwd"));

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid username or password.", result.ErrorMessage);
        
        _currentUserMock.Verify(x => x.SetUser(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()), Times.Never);
    }
}
