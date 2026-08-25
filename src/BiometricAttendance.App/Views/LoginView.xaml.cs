using BiometricAttendance.App.ViewModels;
using System.Windows;

namespace BiometricAttendance.App.Views;

public partial class LoginView : Window
{
    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        viewModel.LoginSucceeded += OnLoginSucceeded;

        // Focus username on load
        Loaded += (_, _) => UsernameBox.Focus();
    }

    private void OnLoginSucceeded()
    {
        // MainWindow is resolved and shown by App.xaml.cs
        DialogResult = true;
        Close();
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
            vm.Password = PasswordBox.Password;
    }
}
