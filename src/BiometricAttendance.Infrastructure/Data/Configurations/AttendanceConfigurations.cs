using BiometricAttendance.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiometricAttendance.Infrastructure.Data.Configurations;

public class AttendanceEventConfiguration : IEntityTypeConfiguration<AttendanceEvent>
{
    public void Configure(EntityTypeBuilder<AttendanceEvent> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.EventType).IsRequired().HasConversion<string>().HasMaxLength(20);
        b.Property(e => e.Source).IsRequired().HasConversion<string>().HasMaxLength(30);
        b.Property(e => e.IdempotencyKey).HasMaxLength(200);

        // Core composite index per 12-DATABASE-SCHEMA.md Section 4
        b.HasIndex(e => new { e.EmployeeId, e.Timestamp });
        // Idempotency index for duplicate prevention (14-OFFLINE-SYNC.md Section 7)
        b.HasIndex(e => e.IdempotencyKey).IsUnique().HasFilter("[IdempotencyKey] IS NOT NULL");

        b.HasOne(e => e.Employee).WithMany(e => e.AttendanceEvents)
            .HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.Device).WithMany(e => e.AttendanceEvents)
            .HasForeignKey(e => e.DeviceId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(20);

        // One record per employee per date
        b.HasIndex(e => new { e.EmployeeId, e.Date }).IsUnique();

        b.HasOne(e => e.Employee).WithMany(e => e.AttendanceRecords)
            .HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.Schedule).WithMany(e => e.AttendanceRecords)
            .HasForeignKey(e => e.ScheduleId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class AttendanceCorrectionConfiguration : IEntityTypeConfiguration<AttendanceCorrection>
{
    public void Configure(EntityTypeBuilder<AttendanceCorrection> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.CorrectionType).IsRequired().HasConversion<string>().HasMaxLength(20);
        b.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        b.Property(e => e.Reason).IsRequired().HasMaxLength(1000);
        b.Property(e => e.OriginalValue).HasMaxLength(500);
        b.Property(e => e.RequestedValue).HasMaxLength(500);
        b.Property(e => e.AttachmentPath).HasMaxLength(500);
        b.Property(e => e.ReviewComment).HasMaxLength(1000);

        b.HasOne(e => e.AttendanceRecord).WithMany(e => e.Corrections)
            .HasForeignKey(e => e.AttendanceRecordId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.Employee).WithMany()
            .HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.RequestedBy).WithMany()
            .HasForeignKey(e => e.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.ReviewedBy).WithMany()
            .HasForeignKey(e => e.ReviewedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}
