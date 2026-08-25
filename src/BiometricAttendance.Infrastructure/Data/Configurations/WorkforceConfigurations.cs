using BiometricAttendance.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiometricAttendance.Infrastructure.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Name).IsRequired().HasMaxLength(200);
        b.Property(e => e.Description).HasMaxLength(500);
        b.HasIndex(e => e.Name).IsUnique();
    }
}

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Name).IsRequired().HasMaxLength(200);
        b.Property(e => e.Description).HasMaxLength(500);
        b.HasIndex(e => e.Name).IsUnique();
    }
}
