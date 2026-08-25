using BiometricAttendance.App.ViewModels;
using System.Windows;

namespace BiometricAttendance.App.Views;

public partial class MainShellView : Window
{
    public MainShellView(MainShellViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.LogoutRequested += OnLogoutRequested;
    }

    private void OnLogoutRequested()
    {
        // App.xaml.cs handles re-showing the login window
        Close();
    }
}
