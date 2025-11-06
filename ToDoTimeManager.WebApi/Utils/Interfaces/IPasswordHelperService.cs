using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Utils.Interfaces;

public interface IPasswordHelperService
{
    string HashPassword(string salt, string password);
    bool VerifyPassword(User user, string hashedPassword);
}