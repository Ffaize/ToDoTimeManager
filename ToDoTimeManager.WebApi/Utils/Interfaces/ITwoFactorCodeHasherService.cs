namespace ToDoTimeManager.WebApi.Utils.Interfaces;

public interface ITwoFactorCodeHasherService
{
    string HashCode(string plainCode);
    bool VerifyCode(string plainCode, string storedHash);
}
