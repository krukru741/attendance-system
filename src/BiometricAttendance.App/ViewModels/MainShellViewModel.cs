using BiometricAttendance.App.ViewModels.Base;
using BiometricAttendance.App.Services;
using BiometricAttendance.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BiometricAttendance.App.ViewModels;

public sealed class MainShellViewModel : ViewModelBase
{
    private readonly NavigationService _navigation;
    private readonly IAuthService _authService;

    private ViewModelBase? _currentPage;
    public ViewModelBase? CurrentPage
    {
        get => _currentPage;
        private set => SetProperty(ref _currentPage, value);
    }

    public SidebarViewModel Sidebar { get; }
    public TopBarViewModel TopBar { get; }

    public event Action? LogoutRequested;

    public MainShellViewModel(
        NavigationService navigation,
        SidebarViewModel sidebar,
        TopBarViewModel topBar,
        IAuthService authService,
        DashboardViewModel dashboard)
    {
        _navigation = navigation;
        _authService = authService;
        Sidebar = sidebar;
        TopBar = topBar;
        TopBar.LogoutRequested += OnLogoutRequested;

        // Subscribe to navigation events
        _navigation.NavigationRequested += vm => CurrentPage = vm;

        // Start on Dashboard
        CurrentPage = dashboard;
    }

    private async void OnLogoutRequested()
    {
        await _authService.LogoutAsync();
        TopBar.Cleanup();
        LogoutRequested?.Invoke();
    }
}
