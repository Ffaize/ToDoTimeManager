using ToDoTimeManager.Entities.Entities;
using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.DataAccess.DataControllers.Interfaces;

public interface IUsersDataController
{
    Task<List<UserEntity>> GetAllUsers();
    Task<UserEntity?> GetUserById(Guid userId);
    Task<UserEntity?> GetUserByUsername(string username);
    Task<UserEntity?> GetUserByEmail(string email);
    Task<UserEntity?> GetUserByLoginParameter(string loginParameter);
    Task<bool> CreateUser(UserEntity newUser);
    Task<bool> UpdateUser(UserEntity updatedUser);
    Task<bool> DeleteUser(Guid userId);
    Task<UserRole?> GetUserRoleByUserId(Guid userId);
}