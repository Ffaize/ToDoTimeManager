using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Shared.Models;

public class NavBarUserModel
{
    public string?   Username { get; set; }
    public string?   Name     { get; set; }
    public UserRole? Role     { get; set; }
    public string?   Avatar   { get; set; }
}
