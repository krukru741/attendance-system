using BiometricAttendance.App.Services;
using BiometricAttendance.App.ViewModels;
using BiometricAttendance.App.Views;
using BiometricAttendance.Application.Interfaces;
using BiometricAttendance.Application.Services;
using BiometricAttendance.Core.Interfaces;
using BiometricAttendance.Infrastructure.Data;
using BiometricAttendance.Infrastructure.Data.Seeds;
using BiometricAttendance.Infrastructure.Repositories;
using BiometricAttendance.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Windows;

namespace BiometricAttendance.App;

public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // ── Configure Serilog ──────────────────────────────────
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BiometricAttendance", "Logs", "app-.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug()
            .WriteTo.File(logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30)
            .CreateLogger();

        // ── Build Host ────────────────────────────────────────
        _host = Host.CreateDefaultBuilder()
            .UseSerilog()
            .ConfigureAppConfiguration(config => config
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true))
            .ConfigureServices(ConfigureServices)
            .Build();

        await _host.StartAsync();

        // ── Migrate & Seed Database ───────────────────────────
        try
        {
            var seeder = _host.Services.GetRequiredService<InitialDataSeeder>();
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Database initialization failed");
            MessageBox.Show(
                $"Failed to initialize the database:\n\n{ex.Message}\n\nThe application will now close.",
                "Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Current.Shutdown(1);
            return;
        }

        // ── Show Login ────────────────────────────────────────
        ShowLoginWindow();
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=attendance.db";

        // EF Core
        services.AddDbContextFactory<AttendanceDbContext>(options =>
            options.UseSqlite("Data Source=attendance.db")
                   .EnableDetailedErrors()
                   .EnableSensitiveDataLogging(false));

        // Infrastructure
        services.AddScoped<BiometricAttendance.Core.Interfaces.IDbContextFactory, AttendanceDbContextFactory>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<InitialDataSeeder>();

        // Application services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IAuditService, AuditService>();

        // App services (singletons)
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<NavigationService>();

        // ViewModels (transient so each window gets fresh instances)
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainShellViewModel>();
        services.AddTransient<SidebarViewModel>();
        services.AddTransient<TopBarViewModel>();
        services.AddTransient<DashboardViewModel>();

        // Views (transient — Windows created and closed)
        services.AddTransient<LoginView>();
        services.AddTransient<MainShellView>();
    }

    private void ShowLoginWindow()
    {
        using var scope = _host!.Services.CreateScope();
        var loginWindow = scope.ServiceProvider.GetRequiredService<LoginView>();

        var result = loginWindow.ShowDialog();

        if (result == true)
        {
            ShowMainShell();
        }
        else
        {
            Current.Shutdown();
        }
    }

    private void ShowMainShell()
    {
        using var scope = _host!.Services.CreateScope();
        var shellWindow = scope.ServiceProvider.GetRequiredService<MainShellView>();
        var shellVm = (MainShellViewModel)shellWindow.DataContext;

        shellVm.LogoutRequested += () =>
        {
            shellWindow.Close();
            // Re-show login after logout
            ShowLoginWindow();
        };

        shellWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
