namespace BiometricAttendance.Core.Interfaces;

/// <summary>
/// Abstraction over password hashing. Implemented by BCrypt in Infrastructure.
/// Keeps the Application layer free of BCrypt dependency.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string plaintext);
    bool Verify(string plaintext, string hash);
}
