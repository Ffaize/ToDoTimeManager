namespace ToDoTimeManager.Business.Utils.Interfaces;

public interface ITwoFactorCodesHelper
{
    string HashCode(string plainCode);
    bool VerifyCode(string plainCode, string storedHash);
    string GenerateCode();
}
