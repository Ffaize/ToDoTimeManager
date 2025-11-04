using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Utils.Interfaces
{
    public interface IPasswordHelperService
    {
        string HashPassword(string salt, string password);
        bool VerifyPassword(User user, string hashedPassword);
    }
}