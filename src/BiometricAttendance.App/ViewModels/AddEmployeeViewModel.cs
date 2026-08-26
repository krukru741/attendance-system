using BiometricAttendance.Application.Interfaces;
using BiometricAttendance.Core.Entities;
using BiometricAttendance.Core.Enums;
using BiometricAttendance.Core.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace BiometricAttendance.App.ViewModels;

public partial class AddEmployeeViewModel : ObservableObject
{
    private readonly IEmployeeService _employeeService;
    private readonly IReferenceRepository _referenceRepository;
    private readonly IEmployeeRepository _employeeRepository;

    [ObservableProperty]
    private int _currentStep = 1;
    public int TotalSteps => 4;

    [ObservableProperty] private string _windowTitle = "Add New Employee";
    private int? _editingEmployeeId;
    
    // UI Lookups
    [ObservableProperty] private ObservableCollection<Department> _departments = new();
    [ObservableProperty] private ObservableCollection<Position> _positions = new();
    [ObservableProperty] private ObservableCollection<ShiftTemplate> _shiftTemplates = new();
    public IEnumerable<EmploymentType> EmploymentTypes => Enum.GetValues<EmploymentType>();
    public IEnumerable<Gender> GenderOptions => Enum.GetValues<Gender>();

    // Step 1: Personal
    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _middleName = string.Empty;
    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _suffix = string.Empty;
    [ObservableProperty] private DateOnly _birthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-20));
    [ObservableProperty] private Gender _gender = Gender.Other;
    [ObservableProperty] private string _phone = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _address = string.Empty;

    // Step 2: Employment
    [ObservableProperty] private string _employeeCode = string.Empty;
    [ObservableProperty] private Department? _selectedDepartment;
    [ObservableProperty] private Position? _selectedPosition;
    [ObservableProperty] private EmploymentType _employmentType = EmploymentType.FullTime;
    [ObservableProperty] private DateOnly _dateHired = DateOnly.FromDateTime(DateTime.Today);

    // Step 3: Settings
    [ObservableProperty] private ShiftTemplate? _defaultShift;
    [ObservableProperty] private int _gracePeriodMinutes = 10;
    [ObservableProperty] private bool _isOvertimeEligible = true;

    // UI Action
    public Action? CloseAction { get; set; }
    public bool? DialogResult { get; set; }

    public AddEmployeeViewModel(
        IEmployeeService employeeService,
        IReferenceRepository referenceRepository,
        IEmployeeRepository employeeRepository)
    {
        _employeeService = employeeService;
        _referenceRepository = referenceRepository;
        _employeeRepository = employeeRepository;
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        Departments = new ObservableCollection<Department>(await _referenceRepository.GetDepartmentsAsync());
        Positions = new ObservableCollection<Position>(await _referenceRepository.GetPositionsAsync());
        ShiftTemplates = new ObservableCollection<ShiftTemplate>(await _referenceRepository.GetShiftTemplatesAsync());

        // Default selections
        if (SelectedDepartment == null) SelectedDepartment = Departments.FirstOrDefault();
        if (SelectedPosition == null) SelectedPosition = Positions.FirstOrDefault();
        if (DefaultShift == null) DefaultShift = ShiftTemplates.FirstOrDefault(s => s.Name == "Regular Shift") ?? ShiftTemplates.FirstOrDefault();
    }

    public void LoadForEdit(int employeeId)
    {
        _editingEmployeeId = employeeId;
        WindowTitle = "Edit Employee";
        // Actually load employee logic would go here. For brevity, assuming the caller sets it or we fetch it.
        _ = LoadEmployeeData(employeeId);
    }

    private async Task LoadEmployeeData(int employeeId)
    {
        await InitializeAsync(); // ensure lookups
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee != null)
        {
            FirstName = employee.FirstName;
            LastName = employee.LastName;
            MiddleName = employee.MiddleName ?? "";
            Suffix = employee.Suffix ?? "";
            BirthDate = employee.BirthDate;
            Gender = employee.Gender;
            Phone = employee.Phone ?? "";
            Email = employee.Email ?? "";
            Address = employee.Address ?? "";

            EmployeeCode = employee.EmployeeCode;
            SelectedDepartment = Departments.FirstOrDefault(d => d.Id == employee.DepartmentId);
            SelectedPosition = Positions.FirstOrDefault(p => p.Id == employee.PositionId);
            EmploymentType = employee.EmploymentType;
            DateHired = employee.DateHired;

            DefaultShift = ShiftTemplates.FirstOrDefault(s => s.Id == employee.DefaultShiftTemplateId);
            GracePeriodMinutes = employee.GracePeriodMinutes;
            IsOvertimeEligible = employee.IsOvertimeEligible;
        }
    }

    [RelayCommand]
    public void NextStep()
    {
        if (CurrentStep < TotalSteps)
        {
            CurrentStep++;
        }
    }

    [RelayCommand]
    public void PreviousStep()
    {
        if (CurrentStep > 1)
        {
            CurrentStep--;
        }
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(EmployeeCode))
            {
                MessageBox.Show("First Name, Last Name, and Employee Code are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var employee = _editingEmployeeId.HasValue 
                ? await _employeeRepository.GetByIdAsync(_editingEmployeeId.Value) 
                : new Employee();

            if (employee == null) return;

            employee.EmployeeCode = EmployeeCode;
            employee.FirstName = FirstName;
            employee.MiddleName = string.IsNullOrWhiteSpace(MiddleName) ? null : MiddleName;
            employee.LastName = LastName;
            employee.Suffix = string.IsNullOrWhiteSpace(Suffix) ? null : Suffix;
            employee.BirthDate = BirthDate;
            employee.Gender = Gender;
            employee.Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone;
            employee.Email = string.IsNullOrWhiteSpace(Email) ? null : Email;
            employee.Address = string.IsNullOrWhiteSpace(Address) ? null : Address;

            employee.DepartmentId = SelectedDepartment?.Id ?? 0;
            employee.PositionId = SelectedPosition?.Id ?? 0;
            employee.EmploymentType = EmploymentType;
            employee.DateHired = DateHired;

            employee.DefaultShiftTemplateId = DefaultShift?.Id;
            employee.GracePeriodMinutes = GracePeriodMinutes;
            employee.IsOvertimeEligible = IsOvertimeEligible;
            
            // Hardcode weekend rest days for now since it wasn't exposed in UI
            employee.RestDays = DayOfWeek.Saturday | DayOfWeek.Sunday;

            if (_editingEmployeeId.HasValue)
            {
                await _employeeService.UpdateEmployeeAsync(employee);
            }
            else
            {
                await _employeeService.CreateEmployeeAsync(employee);
            }

            DialogResult = true;
            CloseAction?.Invoke();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error Saving", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    public void Cancel()
    {
        DialogResult = false;
        CloseAction?.Invoke();
    }
}
