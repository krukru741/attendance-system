using BiometricAttendance.App.Services;
using BiometricAttendance.App.ViewModels.Base;
using BiometricAttendance.Application.Interfaces;
using BiometricAttendance.Core.Interfaces;

namespace BiometricAttendance.App.ViewModels;

public sealed class TopBarViewModel : ViewModelBase
{
    private readonly ICurrentUserService _currentUser;


    // Timer for live clock
    private readonly System.Windows.Threading.DispatcherTimer _clockTimer;

    private string _currentTime = string.Empty;
    private string _currentDate = string.Empty;

    public string DisplayName => _currentUser.CurrentUser?.DisplayName ?? string.Empty;
    public string CurrentTime { get => _currentTime; private set => SetProperty(ref _currentTime, value); }
    public string CurrentDate { get => _currentDate; private set => SetProperty(ref _currentDate, value); }

    public event Action? LogoutRequested;

    public RelayCommand LogoutCommand { get; }

    public TopBarViewModel(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;

        LogoutCommand = new RelayCommand(() => LogoutRequested?.Invoke());

        _clockTimer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _clockTimer.Tick += (_, _) => UpdateClock();
        _clockTimer.Start();
        UpdateClock();
    }

    private void UpdateClock()
    {
        var now = DateTime.Now;
        CurrentTime = now.ToString("hh:mm:ss tt");
        CurrentDate = now.ToString("dddd, MMMM d, yyyy");
    }

    public void Cleanup() => _clockTimer.Stop();
}
