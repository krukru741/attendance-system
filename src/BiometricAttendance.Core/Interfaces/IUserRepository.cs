using BiometricAttendance.Core.Entities;

namespace BiometricAttendance.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<User?> GetByIdWithRolesAsync(int userId, CancellationToken ct = default);
    Task<bool> UsernameExistsAsync(string username, CancellationToken ct = default);
}
