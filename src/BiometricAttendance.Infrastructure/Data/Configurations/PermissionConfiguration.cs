using BiometricAttendance.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiometricAttendance.Infrastructure.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Name).IsRequired().HasMaxLength(100);
        b.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
        b.Property(e => e.Module).IsRequired().HasMaxLength(100);
        b.Property(e => e.Description).HasMaxLength(500);
        b.HasIndex(e => e.Name).IsUnique();
    }
}
