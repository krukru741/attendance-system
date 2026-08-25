using BiometricAttendance.App.Services;
using BiometricAttendance.App.ViewModels.Base;
using BiometricAttendance.Core.Interfaces;
using System.Collections.ObjectModel;

namespace BiometricAttendance.App.ViewModels;

/// <summary>
/// Navigation item shown in the sidebar. Hidden if the user lacks the required permission.
/// </summary>
public sealed class NavItem : ViewModelBase
{
    public string Label { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;  // Unicode icon glyph or path data
    public string? RequiredPermission { get; init; }
    public Type? TargetViewModel { get; init; }

    private bool _isActive;
    public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }
}

public sealed class SidebarViewModel : ViewModelBase
{
    private readonly NavigationService _navigation;
    private readonly ICurrentUserService _currentUser;

    public ObservableCollection<NavItem> NavItems { get; } = new();
    public ObservableCollection<NavItem> AdminNavItems { get; } = new();

    private NavItem? _activeItem;

    public SidebarViewModel(NavigationService navigation, ICurrentUserService currentUser)
    {
        _navigation = navigation;
        _currentUser = currentUser;
        BuildNavItems();
    }

    private void BuildNavItems()
    {
        var allItems = new[]
        {
            new NavItem { Label = "Dashboard",   Icon = "⊞", RequiredPermission = "dashboard.view",  TargetViewModel = typeof(DashboardViewModel) },
        };

        var allAdminItems = new[]
        {
            new NavItem { Label = "Workforce",      Icon = "👥", RequiredPermission = "employee.view" },
            new NavItem { Label = "Biometrics",     Icon = "🔬", RequiredPermission = "biometric.enroll" },
            new NavItem { Label = "Attendance",     Icon = "📋", RequiredPermission = "attendance.view" },
            new NavItem { Label = "Scheduling",     Icon = "📅", RequiredPermission = "schedule.view" },
            new NavItem { Label = "Leave",          Icon = "✈",  RequiredPermission = "leave.view" },
            new NavItem { Label = "Overtime",       Icon = "⏱",  RequiredPermission = "overtime.view" },
            new NavItem { Label = "Reports",        Icon = "📊", RequiredPermission = "reports.view" },
            new NavItem { Label = "Administration", Icon = "🛡",  RequiredPermission = "users.manage" },
            new NavItem { Label = "Settings",       Icon = "⚙",  RequiredPermission = "settings.manage" },
        };

        foreach (var item in allItems.Where(i => i.RequiredPermission is null || _currentUser.HasPermission(i.RequiredPermission)))
            NavItems.Add(item);

        foreach (var item in allAdminItems.Where(i => i.RequiredPermission is null || _currentUser.HasPermission(i.RequiredPermission)))
            AdminNavItems.Add(item);

        // Activate first visible item
        var first = NavItems.FirstOrDefault();
        if (first is not null) SetActive(first);
    }

    public RelayCommand<NavItem> NavigateCommand => new(item =>
    {
        if (item?.TargetViewModel is null) return;
        SetActive(item);
        // Navigation to other VMs will be wired in Phase 2+
    });

    private void SetActive(NavItem item)
    {
        if (_activeItem is not null) _activeItem.IsActive = false;
        _activeItem = item;
        item.IsActive = true;
    }
}
