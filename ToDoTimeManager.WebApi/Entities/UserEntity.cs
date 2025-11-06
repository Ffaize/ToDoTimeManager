using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Entities;

public class UserEntity
{

    public UserEntity()
    {

    }

    public UserEntity(User user)
    {
        Id = user.Id;
        UserName = user.UserName;
        Email = user.Email;
        Password = user.Password;
        UserRole = user.UserRole;
    }

    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public UserRole? UserRole { get; set; }

    public User ToUser()
    {
        return new User
        {
            Id = Id,
            UserName = UserName,
            Email = Email,
            Password = Password,
            UserRole = UserRole
        };
    }
}