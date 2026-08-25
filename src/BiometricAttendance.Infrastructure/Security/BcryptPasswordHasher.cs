using BiometricAttendance.Core.Interfaces;

namespace BiometricAttendance.Infrastructure.Security;

/// <summary>
/// BCrypt-based password hasher. Work factor 12 balances security and performance
/// for an enterprise desktop application.
/// </summary>
public sealed class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string plaintext)
        => BCrypt.Net.BCrypt.HashPassword(plaintext, WorkFactor);

    public bool Verify(string plaintext, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(plaintext, hash);
        }
        catch
        {
            // Invalid hash format — treat as non-matching
            return false;
        }
    }
}
