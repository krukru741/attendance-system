using BiometricAttendance.App.Services;
using BiometricAttendance.Application.Interfaces;
using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BiometricAttendance.App.ViewModels;

public partial class EmployeeProfileViewModel : ObservableObject
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeService _employeeService;
    private readonly ICurrentUserService _currentUser;
    private readonly NavigationService _navigation;

    public bool CanEditEmployee => _currentUser.HasPermission("employee.edit");
    public bool CanArchiveEmployee => _currentUser.HasPermission("employee.archive");

    [ObservableProperty]
    private Employee? _employee;

    public EmployeeProfileViewModel(
        IEmployeeRepository employeeRepository,
        IEmployeeService employeeService,
        ICurrentUserService currentUser,
        NavigationService navigation)
    {
        _employeeRepository = employeeRepository;
        _employeeService = employeeService;
        _currentUser = currentUser;
        _navigation = navigation;
    }

    public void Initialize(int employeeId)
    {
        _ = LoadEmployeeAsync(employeeId);
    }

    private async Task LoadEmployeeAsync(int id)
    {
        Employee = await _employeeRepository.GetByIdAsync(id);
    }

    [RelayCommand]
    public void GoBack()
    {
        _navigation.GoBack();
    }

    [RelayCommand]
    public void EditEmployee()
    {
        if (Employee == null || !CanEditEmployee) return;

        var window = App.Current.Services.GetRequiredService<Views.AddEmployeeView>();
        var vm = (AddEmployeeViewModel)window.DataContext;
        vm.LoadForEdit(Employee.Id);
        
        var result = window.ShowDialog();
        if (result == true)
        {
            _ = LoadEmployeeAsync(Employee.Id);
        }
    }

    [RelayCommand]
    public async Task ArchiveEmployee()
    {
        if (Employee == null || !CanArchiveEmployee) return;

        var msg = MessageBox.Show(
            $"Are you sure you want to archive {Employee.FullName}?",
            "Confirm Archive",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (msg == MessageBoxResult.Yes)
        {
            try
            {
                await _employeeService.ArchiveEmployeeAsync(Employee.Id);
                GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
