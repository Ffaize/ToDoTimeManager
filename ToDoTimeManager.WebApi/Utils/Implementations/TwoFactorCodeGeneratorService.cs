using System.Security.Cryptography;
using ToDoTimeManager.WebApi.Utils.Interfaces;

namespace ToDoTimeManager.WebApi.Utils.Implementations;

public class TwoFactorCodeGeneratorService : ITwoFactorCodeGeneratorService
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public string GenerateCode()
    {
        var chars = new char[6];
        for (var i = 0; i < 6; i++)
            chars[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];

        return $"{chars[0]}{chars[1]}{chars[2]}-{chars[3]}{chars[4]}{chars[5]}";
    }
}
