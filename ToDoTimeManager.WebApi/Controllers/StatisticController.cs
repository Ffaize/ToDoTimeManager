using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoTimeManager.Shared.Models;
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

        private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private bool IsAdmin() => User.IsInRole("Admin");

        [HttpGet("GetToDoCountStatisticsOfAllTimeByUserId/{userId}")]
        public async Task<IActionResult> GetToDoCountStatisticsOfAllTimeByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");
            if (userId != GetCurrentUserId() && !IsAdmin())
                return Forbid();
            var statistics = await _statisticService.GetToDoCountStatisticsOfAllTimeByUserId(userId);
            return Ok(statistics);
        }

        [HttpPost("GetMainPageStatistic")]
        public async Task<IActionResult> GetMainPageStatistic([FromBody] MainPageStatisticRequest filter)
        {
            if (filter.UserId == Guid.Empty)
                return BadRequest("Invalid user ID");
            if (filter.UserId != GetCurrentUserId() && !IsAdmin())
                return Forbid();
            var statistic = await _statisticService.GetMainPageStatistic(filter);
            return Ok(statistic);
        }
    }
}
