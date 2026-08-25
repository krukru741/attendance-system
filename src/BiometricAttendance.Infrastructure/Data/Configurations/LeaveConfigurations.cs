using BiometricAttendance.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiometricAttendance.Infrastructure.Data.Configurations;

public class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Name).IsRequired().HasMaxLength(200);
        b.Property(e => e.Description).HasMaxLength(500);
        b.HasIndex(e => e.Name).IsUnique();
    }
}

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        b.Property(e => e.Reason).IsRequired().HasMaxLength(1000);
        b.Property(e => e.AttachmentPath).HasMaxLength(500);
        b.Property(e => e.ReviewComment).HasMaxLength(1000);

        b.HasIndex(e => new { e.EmployeeId, e.StartDate });

        b.HasOne(e => e.Employee).WithMany(e => e.LeaveRequests)
            .HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.LeaveType).WithMany(e => e.LeaveRequests)
            .HasForeignKey(e => e.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.SubmittedBy).WithMany()
            .HasForeignKey(e => e.SubmittedByUserId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.ReviewedBy).WithMany()
            .HasForeignKey(e => e.ReviewedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class LeaveBalanceConfiguration : IEntityTypeConfiguration<LeaveBalance>
{
    public void Configure(EntityTypeBuilder<LeaveBalance> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Entitled).HasPrecision(6, 2);
        b.Property(e => e.Used).HasPrecision(6, 2);
        b.Ignore(e => e.Remaining); // computed

        b.HasIndex(e => new { e.EmployeeId, e.LeaveTypeId, e.Year }).IsUnique();

        b.HasOne(e => e.Employee).WithMany()
            .HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.LeaveType).WithMany(e => e.LeaveBalances)
            .HasForeignKey(e => e.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}
