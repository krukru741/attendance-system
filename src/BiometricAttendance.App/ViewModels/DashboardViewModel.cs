using BiometricAttendance.App.ViewModels.Base;
using BiometricAttendance.Core.Interfaces;

namespace BiometricAttendance.App.ViewModels;

/// <summary>
/// Dashboard stub for Phase 1.
/// KPI data will be populated in later phases when the employee + attendance modules are built.
/// Quick actions are only visible when the user holds the corresponding module permission.
/// </summary>
public sealed class DashboardViewModel : ViewModelBase
{
    private readonly ICurrentUserService _currentUser;

    // ── KPI Stats (stub data for Phase 1) ──
    public int TotalEmployees   { get; private set; } = 142;
    public int PresentToday     { get; private set; } = 120;
    public int AbsentToday      { get; private set; } = 5;
    public int LateToday        { get; private set; } = 12;
    public int OnLeaveToday     { get; private set; } = 5;
    public int OvertimeToday    { get; private set; } = 3;

    // ── Permission-gated quick actions ──
    public bool CanAddEmployee   => _currentUser.HasPermission("employee.create");
    public bool CanEnrollBio     => _currentUser.HasPermission("biometric.enroll");
    public bool CanViewAttendance => _currentUser.HasPermission("attendance.view");
    public bool CanReviewLeave   => _currentUser.HasPermission("leave.approve") || _currentUser.HasPermission("overtime.approve");

    public string GreetingText { get; private set; } = string.Empty;

    public DashboardViewModel(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
        GreetingText = GetGreeting();
    }

    private static string GetGreeting()
    {
        var hour = DateTime.Now.Hour;
        return hour switch
        {
            < 12 => "Good morning",
            < 17 => "Good afternoon",
            _    => "Good evening"
        };
    }
}
