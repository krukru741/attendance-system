using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BiometricAttendance.Infrastructure.Data;

/// <summary>
/// Enables `dotnet ef migrations add` / `dotnet ef database update` from the CLI
/// without launching the WPF host application.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AttendanceDbContext>
{
    public AttendanceDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? "Data Source=attendance.db";

        var optionsBuilder = new DbContextOptionsBuilder<AttendanceDbContext>();
        optionsBuilder.UseSqlite(connectionString);

        return new AttendanceDbContext(optionsBuilder.Options);
    }
}
