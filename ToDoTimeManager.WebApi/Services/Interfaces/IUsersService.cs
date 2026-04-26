using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IUsersService
{
    Task<List<User>> GetAllUsers();
    Task<User?> GetUserById(Guid userId, Guid currentUserId, bool isAdmin);
    Task<User?> GetUserByUsername(string username);
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserByLoginParameter(string loginParameter);
    Task<bool> CreateUser(CreateUserRequestDto request);
    Task<bool> UpdateUser(UpdateUserRequestDto request, Guid currentUserId);
    Task<bool> ChangeUserRole(Guid userId, UserRole newRole);
    Task<bool> DeleteUser(Guid userId);
}
