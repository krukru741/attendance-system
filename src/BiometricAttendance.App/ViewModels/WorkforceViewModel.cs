using BiometricAttendance.App.Services;
using BiometricAttendance.Application.Interfaces;
using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Enums;
using BiometricAttendance.Core.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace BiometricAttendance.App.ViewModels;

public partial class WorkforceViewModel : ObservableObject
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeService _employeeService;
    private readonly IReferenceRepository _referenceRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly NavigationService _navigation;
    
    // RBAC
    public bool CanAddEmployee => _currentUser.HasPermission("employee.create");
    public bool CanEditEmployee => _currentUser.HasPermission("employee.edit");
    public bool CanArchiveEmployee => _currentUser.HasPermission("employee.archive");
    public bool CanViewEmployee => _currentUser.HasPermission("employee.view");

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new();

    [ObservableProperty]
    private ObservableCollection<Department> _departments = new();

    // Filters
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private Department? _selectedDepartment;

    [ObservableProperty]
    private EmploymentStatus? _selectedStatus = EmploymentStatus.Active;

    public IEnumerable<EmploymentStatus> StatusOptions => Enum.GetValues<EmploymentStatus>();

    public WorkforceViewModel(
        IEmployeeRepository employeeRepository,
        IEmployeeService employeeService,
        IReferenceRepository referenceRepository,
        ICurrentUserService currentUser,
        NavigationService navigation)
    {
        _employeeRepository = employeeRepository;
        _employeeService = employeeService;
        _referenceRepository = referenceRepository;
        _currentUser = currentUser;
        _navigation = navigation;
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        if (!CanViewEmployee)
            return;

        var deps = await _referenceRepository.GetDepartmentsAsync();
        Departments = new ObservableCollection<Department>(deps);

        await LoadEmployeesAsync();
    }

    [RelayCommand]
    public async Task LoadEmployeesAsync()
    {
        if (!CanViewEmployee) return;

        var employees = await _employeeRepository.GetFilteredAsync(
            SearchText,
            SelectedDepartment?.Id,
            SelectedStatus);

        Employees = new ObservableCollection<Employee>(employees);
    }

    // Property change triggers reload
    partial void OnSearchTextChanged(string value) => _ = LoadEmployeesAsync();
    partial void OnSelectedDepartmentChanged(Department? value) => _ = LoadEmployeesAsync();
    partial void OnSelectedStatusChanged(EmploymentStatus? value) => _ = LoadEmployeesAsync();

    [RelayCommand]
    public void AddEmployee()
    {
        if (!CanAddEmployee) return;

        // In a real WPF app with DI, this would be injected via a factory,
        // but since we are keeping it simple:
        var window = App.Current.Services.GetRequiredService<Views.AddEmployeeView>();
        var result = window.ShowDialog();
        if (result == true)
        {
            _ = LoadEmployeesAsync();
        }
    }

    [RelayCommand]
    public void ViewProfile(Employee? employee)
    {
        if (employee == null || !CanViewEmployee) return;

        var profileVm = App.Current.Services.GetRequiredService<EmployeeProfileViewModel>();
        profileVm.Initialize(employee.Id);
        _navigation.NavigateTo(profileVm);
    }

    [RelayCommand]
    public void EditEmployee(Employee? employee)
    {
        if (employee == null || !CanEditEmployee) return;

        // Open AddEmployeeView in Edit mode
        var window = App.Current.Services.GetRequiredService<Views.AddEmployeeView>();
        var vm = (AddEmployeeViewModel)window.DataContext;
        vm.LoadForEdit(employee.Id);
        
        var result = window.ShowDialog();
        if (result == true)
        {
            _ = LoadEmployeesAsync();
        }
    }

    [RelayCommand]
    public async Task ArchiveEmployee(Employee? employee)
    {
        if (employee == null || !CanArchiveEmployee) return;

        var msg = MessageBox.Show(
            $"Are you sure you want to archive {employee.FullName}? They will be hidden from active lists.",
            "Confirm Archive",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (msg == MessageBoxResult.Yes)
        {
            try
            {
                await _employeeService.ArchiveEmployeeAsync(employee.Id);
                await LoadEmployeesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
