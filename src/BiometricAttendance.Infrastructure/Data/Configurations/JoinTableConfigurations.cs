using BiometricAttendance.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiometricAttendance.Infrastructure.Data.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> b)
    {
        b.HasKey(e => new { e.UserId, e.RoleId });
        b.Property(e => e.AssignedAt).IsRequired();
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> b)
    {
        b.HasKey(e => new { e.RoleId, e.PermissionId });

        b.HasOne(e => e.Permission).WithMany(e => e.RolePermissions)
            .HasForeignKey(e => e.PermissionId).OnDelete(DeleteBehavior.Cascade);
    }
}
