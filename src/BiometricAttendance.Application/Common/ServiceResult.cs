namespace BiometricAttendance.Application.Common;

/// <summary>
/// Typed result wrapper for application service operations.
/// Avoids throwing exceptions for predictable business rule failures
/// (e.g. wrong password, already archived) while keeping exceptions
/// for truly unexpected infrastructure failures.
/// </summary>
public sealed class ServiceResult
{
    public bool IsSuccess { get; private init; }
    public string? ErrorMessage { get; private init; }
    public string? ErrorCode { get; private init; }

    public static ServiceResult Success() => new() { IsSuccess = true };

    public static ServiceResult Failure(string message, string? code = null) =>
        new() { IsSuccess = false, ErrorMessage = message, ErrorCode = code };
}

public sealed class ServiceResult<T>
{
    public bool IsSuccess { get; private init; }
    public T? Value { get; private init; }
    public string? ErrorMessage { get; private init; }
    public string? ErrorCode { get; private init; }

    public static ServiceResult<T> Success(T value) =>
        new() { IsSuccess = true, Value = value };

    public static ServiceResult<T> Failure(string message, string? code = null) =>
        new() { IsSuccess = false, ErrorMessage = message, ErrorCode = code };
}
