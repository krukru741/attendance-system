using BiometricAttendance.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiometricAttendance.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(50);
        b.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        b.Property(e => e.MiddleName).HasMaxLength(100);
        b.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        b.Property(e => e.Suffix).HasMaxLength(20);
        b.Property(e => e.Gender).HasMaxLength(20);
        b.Property(e => e.Phone).HasMaxLength(50);
        b.Property(e => e.Email).HasMaxLength(200);
        b.Property(e => e.Address).HasMaxLength(500);
        b.Property(e => e.PhotoPath).HasMaxLength(500);
        b.Property(e => e.EmploymentType).HasMaxLength(50);
        b.Property(e => e.RestDays).HasMaxLength(20);

        // Lifecycle driven by enum — no boolean lifecycle flags on this entity (Decision 1)
        b.Property(e => e.EmploymentStatus).IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Index on EmployeeCode (unique) and DepartmentId (frequent filter)
        b.HasIndex(e => e.EmployeeCode).IsUnique();
        b.HasIndex(e => e.DepartmentId);

        // Ignore computed property
        b.Ignore(e => e.FullName);

        b.HasOne(e => e.Department).WithMany(e => e.Employees)
            .HasForeignKey(e => e.DepartmentId).OnDelete(DeleteBehavior.SetNull);

        b.HasOne(e => e.Position).WithMany(e => e.Employees)
            .HasForeignKey(e => e.PositionId).OnDelete(DeleteBehavior.SetNull);

        b.HasOne(e => e.Supervisor).WithMany()
            .HasForeignKey(e => e.SupervisorId).OnDelete(DeleteBehavior.SetNull);

        b.HasOne(e => e.DefaultShiftTemplate).WithMany()
            .HasForeignKey(e => e.DefaultShiftTemplateId).OnDelete(DeleteBehavior.SetNull);
    }
}
