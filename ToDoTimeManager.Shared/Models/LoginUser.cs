using System.Linq.Expressions;

namespace ToDoTimeManager.Shared.Models;

public class LoginUser
{
    public string? LoginParameter { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
}