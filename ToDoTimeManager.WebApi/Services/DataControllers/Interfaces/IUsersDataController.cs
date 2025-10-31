using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

public interface IUsersDataController
{
    Task<List<UserEntity>> GetAllUsers();
    Task<UserEntity?> GetUserById(Guid userId);
    Task<UserEntity?> GetUserByUsername(string username);
    Task<bool> CreateUser(UserEntity newUser);
    Task<bool> UpdateUser(UserEntity updatedUser);
    Task<bool> DeleteUser(Guid userId);
}