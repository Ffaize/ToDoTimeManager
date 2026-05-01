using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IUsersService
{
    Task<List<User>> GetAllUsers();
    Task<User?> GetUserById(Guid userId, Guid currentUserId, UserRole currentUserRole);
    Task<User?> GetUserByUsername(string username, Guid currentUserId, UserRole currentUserRole);
    Task<User?> GetUserByEmail(string email, Guid currentUserId, UserRole currentUserRole);
    Task<User?> GetUserByLoginParameter(string loginParameter, Guid currentUserId, UserRole currentUserRole);
    Task<bool> CreateUser(CreateUserRequestDto request);
    Task<bool> UpdateUser(UpdateUserRequestDto request, Guid currentUserId);
    Task<bool> ChangeUserRole(Guid userId, UserRole newRole);
    Task<bool> DeleteUser(Guid userId);
}
