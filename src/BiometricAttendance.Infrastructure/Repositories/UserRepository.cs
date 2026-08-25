using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Interfaces;
using BiometricAttendance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiometricAttendance.Infrastructure.Repositories;

public sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AttendanceDbContext db) : base(db) { }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        => await _db.Users
            .FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<User?> GetByIdWithRolesAsync(int userId, CancellationToken ct = default)
        => await _db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken ct = default)
        => await _db.Users.AnyAsync(u => u.Username == username, ct);
}
