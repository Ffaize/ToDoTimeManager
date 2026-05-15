namespace ToDoTimeManager.Business.Utils.Interfaces;

public interface IPasswordHelperService
{
    string GenerateSalt();
    string HashPassword(string salt, string password);
    bool VerifyPassword(string plainPassword, string storedHash, string storedSalt);
}
