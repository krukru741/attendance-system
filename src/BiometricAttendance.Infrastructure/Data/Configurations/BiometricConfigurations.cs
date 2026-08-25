using BiometricAttendance.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiometricAttendance.Infrastructure.Data.Configurations;

/// <summary>
/// BiometricDevice: Status (operational state enum) and IsActive (soft-delete flag) 
/// are two independent fields with non-overlapping meanings (Decision 2).
/// </summary>
public class BiometricDeviceConfiguration : IEntityTypeConfiguration<BiometricDevice>
{
    public void Configure(EntityTypeBuilder<BiometricDevice> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.DeviceName).IsRequired().HasMaxLength(200);
        b.Property(e => e.IPAddress).HasMaxLength(50);
        b.Property(e => e.Model).HasMaxLength(100);
        b.Property(e => e.SerialNumber).HasMaxLength(100);
        b.Property(e => e.FirmwareVersion).HasMaxLength(50);
        b.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        // IsActive is a plain bool — no special config needed
        b.HasIndex(e => e.Status);
        b.HasIndex(e => e.IsActive);
    }
}

public class EmployeeBiometricConfiguration : IEntityTypeConfiguration<EmployeeBiometric>
{
    public void Configure(EntityTypeBuilder<EmployeeBiometric> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Template).IsRequired();
        b.Property(e => e.TemplateFormat).IsRequired().HasMaxLength(50);
        b.Property(e => e.FingerType).IsRequired().HasConversion<string>().HasMaxLength(30);

        // One employee cannot have two active templates for the same finger
        b.HasIndex(e => new { e.EmployeeId, e.FingerType }).HasFilter("[IsActive] = 1");

        b.HasOne(e => e.Employee).WithMany(e => e.Biometrics)
            .HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.Device).WithMany()
            .HasForeignKey(e => e.DeviceId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class BiometricLogConfiguration : IEntityTypeConfiguration<BiometricLog>
{
    public void Configure(EntityTypeBuilder<BiometricLog> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.LogType).IsRequired().HasConversion<string>().HasMaxLength(30);
        b.Property(e => e.Message).HasMaxLength(1000);
        b.HasIndex(e => e.Timestamp);

        b.HasOne(e => e.Device).WithMany(e => e.Logs)
            .HasForeignKey(e => e.DeviceId).OnDelete(DeleteBehavior.Restrict);
    }
}
