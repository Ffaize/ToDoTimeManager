using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StatisticController : ControllerBase
    {
        private readonly ILogger<StatisticController> _logger;
        private readonly IStatisticService _statisticService;
        public StatisticController(ILogger<StatisticController> logger, IStatisticService statisticService)
        {
            _logger = logger;
            _statisticService = statisticService;
        }

        [HttpGet("GetToDoCountStatisticsOfAllTimeByUserId/{userId}")]
        public async Task<IActionResult> GetToDoCountStatisticsOfAllTimeByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");
            var statistics = await _statisticService.GetToDoCountStatisticsOfAllTimeByUserId(userId);
            return Ok(statistics);
        }
    }
}
