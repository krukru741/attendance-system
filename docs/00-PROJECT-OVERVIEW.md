# Biometric Attendance Tracking System --- Project Overview

## 1. Purpose

A C#-based biometric attendance tracking system for managing employees,
biometric identity, attendance events, schedules, leave, overtime,
reports, users, permissions, audit logs, and database administration.

## 2. Recommended Stack

-   C# / .NET 8+
-   WPF
-   MVVM
-   Entity Framework Core
-   SQL Server
-   Biometric SDK/device integration
-   QuestPDF or RDLC for reports
-   Excel export
-   Dependency Injection
-   Serilog or equivalent structured logging

## 3. High-Level Architecture

``` text
WPF UI
  ↓
ViewModels
  ↓
Application Services
  ↓
Domain/Core
  ↓
Repositories / EF Core
  ↓
SQL Server

Biometric Device
  ↓
Biometric Service / Adapter
  ↓
Attendance Event
  ↓
Attendance Processing Engine
  ↓
Attendance Record
```

## 4. Core Modules

1.  Authentication and Authorization
2.  Dashboard
3.  Workforce / Employees
4.  Biometrics
5.  Attendance
6.  Scheduling
7.  Leave
8.  Overtime
9.  Reports
10. Administration
11. Audit Logging
12. Backup and Restore

## 5. Primary Design Principle

The biometric device should identify **who scanned and when**. The
attendance engine should determine **what that scan means**.

This keeps biometric integration independent from attendance rules and
allows different scanner brands to be supported later.
