using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using ToDoTimeManager.WebApi.Utils.Interfaces;

namespace ToDoTimeManager.WebApi.Utils.Implementations;

public class TwoFactorCodeHasherService : ITwoFactorCodeHasherService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;
    private const char Separator = '.';

    public string HashCode(string plainCode)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = KeyDerivation.Pbkdf2(
            plainCode.ToUpperInvariant(), salt,
            KeyDerivationPrf.HMACSHA256, Iterations, HashSize);
        return $"{Convert.ToBase64String(salt)}{Separator}{Convert.ToBase64String(hash)}";
    }

    public bool VerifyCode(string plainCode, string storedHash)
    {
        var parts = storedHash.Split(Separator, 2);
        if (parts.Length != 2) return false;
        try
        {
            var salt = Convert.FromBase64String(parts[0]);
            var expected = Convert.FromBase64String(parts[1]);
            var actual = KeyDerivation.Pbkdf2(
                plainCode.ToUpperInvariant(), salt,
                KeyDerivationPrf.HMACSHA256, Iterations, HashSize);
            return CryptographicOperations.FixedTimeEquals(expected, actual);
        }
        catch
        {
            return false;
        }
    }
}
