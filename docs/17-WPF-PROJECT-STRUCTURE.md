# C# WPF Project Structure

## Recommended Solution

``` text
BiometricAttendance.sln

BiometricAttendance.App/
├── Views/
├── ViewModels/
├── Controls/
├── Resources/
├── Styles/
├── Converters/
└── App.xaml

BiometricAttendance.Core/
├── Entities/
├── Enums/
├── Interfaces/
├── DTOs/
└── Exceptions/

BiometricAttendance.Application/
├── Services/
├── Interfaces/
├── Validators/
├── Attendance/
└── UseCases/

BiometricAttendance.Infrastructure/
├── Data/
├── Repositories/
├── Biometric/
├── Security/
├── Logging/
└── Sync/

BiometricAttendance.Tests/
├── Unit/
├── Integration/
└── Attendance/
```

## MVVM Flow

``` text
View
 ↓
ViewModel
 ↓
Application Service
 ↓
Core Interface
 ↓
Infrastructure Implementation
 ↓
Database / Device
```

## Rules

-   Views should contain UI concerns only.
-   ViewModels should expose state and commands.
-   Business rules belong in application/domain services.
-   Database access belongs in Infrastructure logic (EF Core SQLite context, Repository implementations, Biometric SDK wrappers).
-   Avoid SQL queries directly inside button click handlers.

## Example Command Flow

``` text
Enroll Button
 ↓
EnrollmentViewModel
 ↓
IBiometricService
 ↓
Biometric Adapter
 ↓
Vendor SDK
 ↓
Template
 ↓
EmployeeBiometric Repository
 ↓
SQL Server
```
