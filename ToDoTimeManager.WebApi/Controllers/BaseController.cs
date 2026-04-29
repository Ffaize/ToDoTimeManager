using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ToDoTimeManager.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    protected bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }
}
