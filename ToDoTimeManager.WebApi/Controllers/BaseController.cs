using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    protected UserRole GetCurrentUserRole()
    {
        var roleClaim = User.FindFirstValue(ClaimTypes.Role);
        return Enum.TryParse<UserRole>(roleClaim, out var role) ? role : UserRole.User;
    }

    protected bool IsAdmin()          => GetCurrentUserRole() >= UserRole.Admin;
    protected bool IsManager()        => GetCurrentUserRole() >= UserRole.Manager;
    protected bool IsProjectManager() => GetCurrentUserRole() >= UserRole.ProjectManager;
    protected bool IsDeveloper()      => GetCurrentUserRole() >= UserRole.Developer;
}
