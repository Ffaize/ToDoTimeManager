using System.Text.Json;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Utils.Interfaces;

namespace ToDoTimeManager.WebApi.Utils.Implementations
{
    public class PasswordHelperService : IPasswordHelperService
    {

        public string HashPassword(UserEntity user, string password)
        {
            if (user.UserName == null) throw new ArgumentNullException("Username cannot be null");
            var userHash = HashPasswordWithSalt(password, DateToByteArray(user.UserName));
            return userHash;
        }

        public bool VerifyPassword(UserEntity user, string hashedPassword)
        {
            var result = VerifyHashedPassword(user, hashedPassword);
            return result == PasswordVerificationResult.Success;
        }

        private static PasswordVerificationResult VerifyHashedPassword(UserEntity user, string hashedPassword)
        {
            return user.Password != null && user.Password.Equals(hashedPassword) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        private static byte[] DateToByteArray(string salt)
        {
            using var memoryStream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(memoryStream))
            {
                writer.WriteStartObject();
                writer.WriteString("string", salt);
                writer.WriteEndObject();
            }
            var dateTimeBytes = memoryStream.ToArray();
            return dateTimeBytes;
        }

        private static string HashPasswordWithSalt(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
        }
    }
}
