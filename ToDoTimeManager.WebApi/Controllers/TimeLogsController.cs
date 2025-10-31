using Microsoft.AspNetCore.Mvc;

namespace ToDoTimeManager.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeLogsController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
    }
}
