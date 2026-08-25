using BiometricAttendance.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiometricAttendance.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Name).IsRequired().HasMaxLength(100);
        b.Property(e => e.Description).HasMaxLength(500);
        b.HasIndex(e => e.Name).IsUnique();

        b.HasMany(e => e.UserRoles).WithOne(e => e.Role)
            .HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Restrict);

        b.HasMany(e => e.RolePermissions).WithOne(e => e.Role)
            .HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}
