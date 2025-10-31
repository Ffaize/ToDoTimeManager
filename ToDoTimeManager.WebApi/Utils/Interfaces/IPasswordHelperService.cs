using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Utils.Interfaces
{
    public interface IPasswordHelperService
    {
        string HashPassword(UserEntity user, string password);
        bool VerifyPassword(UserEntity user, string hashedPassword);
    }
}