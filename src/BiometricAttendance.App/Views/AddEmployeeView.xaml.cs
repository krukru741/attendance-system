using System.Windows;

namespace BiometricAttendance.App.Views;

public partial class AddEmployeeView : Window
{
    public AddEmployeeView(ViewModels.AddEmployeeViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseAction = () =>
        {
            DialogResult = viewModel.DialogResult;
            Close();
        };
    }
}
