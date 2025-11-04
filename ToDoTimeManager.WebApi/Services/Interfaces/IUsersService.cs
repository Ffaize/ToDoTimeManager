using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IUsersService
{
    Task<List<User>> GetAllUsers();
    Task<User?> GetUserById(Guid userId);
    Task<User?> GetUserByUsername(string username);
    Task<bool> CreateUser(User newUser);
    Task<bool> UpdateUser(User updatedUser);
    Task<bool> DeleteUser(Guid userId);
}