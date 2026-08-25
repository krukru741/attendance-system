using BiometricAttendance.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiometricAttendance.Infrastructure.Data.Configurations;

public class ShiftTemplateConfiguration : IEntityTypeConfiguration<ShiftTemplate>
{
    public void Configure(EntityTypeBuilder<ShiftTemplate> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Name).IsRequired().HasMaxLength(200);
        b.HasIndex(e => e.Name).IsUnique();
    }
}

public class EmployeeScheduleConfiguration : IEntityTypeConfiguration<EmployeeSchedule>
{
    public void Configure(EntityTypeBuilder<EmployeeSchedule> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.ScheduleStatus).HasMaxLength(30);

        // Core index per 12-DATABASE-SCHEMA.md Section 4
        b.HasIndex(e => new { e.EmployeeId, e.Date });

        b.HasOne(e => e.Employee).WithMany(e => e.Schedules)
            .HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.ShiftTemplate).WithMany(e => e.Schedules)
            .HasForeignKey(e => e.ShiftTemplateId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
{
    public void Configure(EntityTypeBuilder<Holiday> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Name).IsRequired().HasMaxLength(200);
        b.Property(e => e.Type).IsRequired().HasConversion<string>().HasMaxLength(40);
        b.Property(e => e.Notes).HasMaxLength(500);
        b.HasIndex(e => e.Date);
    }
}
