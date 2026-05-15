using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using ToDoTimeManager.Business.Utils.Interfaces;

namespace ToDoTimeManager.Business.Utils.Implementations;

public class PasswordHelperService : IPasswordHelperService
{
    private const int Iterations = 100_000;
    private const int HashSize = 32;

    public string GenerateSalt() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    public string HashPassword(string salt, string password)
    {
        var saltBytes = Convert.FromBase64String(salt);
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password, saltBytes, KeyDerivationPrf.HMACSHA256, Iterations, HashSize));
    }

    public bool VerifyPassword(string plainPassword, string storedHash, string storedSalt)
    {
        try
        {
            var expected = Convert.FromBase64String(storedHash);
            var saltBytes = Convert.FromBase64String(storedSalt);
            var actual = KeyDerivation.Pbkdf2(
                plainPassword, saltBytes, KeyDerivationPrf.HMACSHA256, Iterations, HashSize);
            return CryptographicOperations.FixedTimeEquals(expected, actual);
        }
        catch
        {
            return false;
        }
    }
}
