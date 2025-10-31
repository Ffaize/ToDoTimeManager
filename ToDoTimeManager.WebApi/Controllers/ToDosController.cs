using Microsoft.AspNetCore.Mvc;

namespace ToDoTimeManager.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDosController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
    }
}
