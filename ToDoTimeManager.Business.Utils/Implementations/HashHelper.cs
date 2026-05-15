using System.Security.Cryptography;
using System.Text;

namespace ToDoTimeManager.Business.Utils.Implementations;

public static class HashHelper
{
    public static string HashRefreshToken(string plainToken) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(plainToken)));
}
