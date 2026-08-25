using BiometricAttendance.App.Services;
using BiometricAttendance.Core.Entities;

namespace BiometricAttendance.Tests.Unit;

public class CurrentUserServiceTests
{
    [Fact]
    public void HasPermission_UserHasPermission_ReturnsTrue()
    {
        // Arrange
        var user = new User { Username = "admin", PasswordHash = "pwd", DisplayName = "Admin", Email = "a@a.c" };
        var service = new CurrentUserService();
        service.SetUser(user, new[] { "employee.create", "settings.manage" });

        // Act
        var result = service.HasPermission("employee.create");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasPermission_UserDoesNotHavePermission_ReturnsFalse()
    {
        // Arrange
        var user = new User { Username = "kiosk", PasswordHash = "pwd", DisplayName = "Kiosk", Email = "a@a.c" };
        var service = new CurrentUserService();
        service.SetUser(user, new[] { "attendance.kiosk" });

        // Act
        var result = service.HasPermission("employee.create");

        // Assert
        Assert.False(result);
    }
}
