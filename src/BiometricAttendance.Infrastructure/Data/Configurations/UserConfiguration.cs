using BiometricAttendance.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BiometricAttendance.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Username).IsRequired().HasMaxLength(100);
        b.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
        b.Property(e => e.Email).HasMaxLength(200);
        b.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
        b.HasIndex(e => e.Username).IsUnique();

        b.HasMany(e => e.UserRoles).WithOne(e => e.User)
            .HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);

        b.HasMany(e => e.AuditLogs).WithOne(e => e.User)
            .HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.SetNull);
    }
}
