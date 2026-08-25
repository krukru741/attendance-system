using BiometricAttendance.App.Services;
using BiometricAttendance.App.ViewModels.Base;
using BiometricAttendance.Application.Interfaces;
using BiometricAttendance.Core.DTOs;
using System.Windows;

namespace BiometricAttendance.App.ViewModels;

public sealed class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly NavigationService _navigation;

    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private bool _isPasswordVisible;

    public string Username
    {
        get => _username;
        set { SetProperty(ref _username, value); LoginCommand.RaiseCanExecuteChanged(); }
    }

    public string Password
    {
        get => _password;
        set { SetProperty(ref _password, value); LoginCommand.RaiseCanExecuteChanged(); }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set { SetProperty(ref _isLoading, value); LoginCommand.RaiseCanExecuteChanged(); }
    }

    public bool IsPasswordVisible
    {
        get => _isPasswordVisible;
        set => SetProperty(ref _isPasswordVisible, value);
    }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public RelayCommand LoginCommand { get; }
    public RelayCommand TogglePasswordVisibilityCommand { get; }

    public event Action? LoginSucceeded;

    public LoginViewModel(IAuthService authService, NavigationService navigation)
    {
        _authService = authService;
        _navigation = navigation;

        LoginCommand = new RelayCommand(
            execute: async () => await LoginAsync(),
            canExecute: () => !IsLoading && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password));

        TogglePasswordVisibilityCommand = new RelayCommand(
            () => IsPasswordVisible = !IsPasswordVisible);
    }

    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        IsLoading = true;

        try
        {
            var result = await _authService.LoginAsync(new LoginRequest(Username.Trim(), Password));

            if (result.Success)
            {
                LoginSucceeded?.Invoke();
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Login failed.";
                OnPropertyChanged(nameof(HasError));
            }
        }
        catch (Exception)
        {
            ErrorMessage = "An unexpected error occurred. Please try again.";
            OnPropertyChanged(nameof(HasError));
        }
        finally
        {
            IsLoading = false;
            Password = string.Empty;
        }
    }
}
