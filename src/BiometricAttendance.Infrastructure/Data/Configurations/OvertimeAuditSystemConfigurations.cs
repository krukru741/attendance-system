using BiometricAttendance.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiometricAttendance.Infrastructure.Data.Configurations;

public class OvertimeRequestConfiguration : IEntityTypeConfiguration<OvertimeRequest>
{
    public void Configure(EntityTypeBuilder<OvertimeRequest> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        b.Property(e => e.Reason).IsRequired().HasMaxLength(1000);
        b.Property(e => e.AttachmentPath).HasMaxLength(500);
        b.Property(e => e.ReviewComment).HasMaxLength(1000);

        b.HasIndex(e => new { e.EmployeeId, e.Date });

        b.HasOne(e => e.Employee).WithMany(e => e.OvertimeRequests)
            .HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.ApprovedBy).WithMany()
            .HasForeignKey(e => e.ApprovedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Action).IsRequired().HasMaxLength(100);
        b.Property(e => e.Module).IsRequired().HasMaxLength(100);
        b.Property(e => e.EntityName).HasMaxLength(100);
        b.Property(e => e.EntityId).HasMaxLength(50);
        b.Property(e => e.MachineName).HasMaxLength(100);
        b.HasIndex(e => e.Timestamp);
        b.HasIndex(e => new { e.UserId, e.Timestamp });
    }
}

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Key).IsRequired().HasMaxLength(200);
        b.Property(e => e.Value).IsRequired().HasMaxLength(2000);
        b.Property(e => e.Description).HasMaxLength(500);
        b.Property(e => e.Category).HasMaxLength(100);
        b.HasIndex(e => e.Key).IsUnique();
    }
}
