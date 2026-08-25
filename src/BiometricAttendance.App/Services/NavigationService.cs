using BiometricAttendance.App.ViewModels;
using BiometricAttendance.App.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;

namespace BiometricAttendance.App.Services;

/// <summary>
/// ViewModel-first navigation service.
/// Resolves ViewModels from DI and fires them into the shell's content area.
/// The shell maps VM types to View types via DataTemplates in App.xaml.
/// </summary>
public sealed class NavigationService
{
    private readonly IServiceProvider _services;

    public event Action<ViewModelBase>? NavigationRequested;

    public NavigationService(IServiceProvider services)
        => _services = services;

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        var vm = _services.GetRequiredService<TViewModel>();
        NavigationRequested?.Invoke(vm);
    }

    public void NavigateTo<TViewModel>(Action<TViewModel> configure) where TViewModel : ViewModelBase
    {
        var vm = _services.GetRequiredService<TViewModel>();
        configure(vm);
        NavigationRequested?.Invoke(vm);
    }
}
