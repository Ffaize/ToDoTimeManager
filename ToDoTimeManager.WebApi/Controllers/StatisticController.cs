using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticController : ControllerBase
{
    private readonly IStatisticService _statisticService;

    public StatisticController(IStatisticService statisticService)
    {
        _statisticService = statisticService;
    }

    private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsAdmin() => User.IsInRole("Admin");

    [HttpGet("GetToDoCountStatisticsOfAllTimeByUserId/{userId}")]
    public async Task<IActionResult> GetToDoCountStatisticsOfAllTimeByUserId(Guid userId)
    {
        var statistics = await _statisticService.GetToDoCountStatisticsOfAllTimeByUserId(userId, GetCurrentUserId(), IsAdmin());
        return Ok(statistics);
    }

    [HttpPost("GetMainPageStatistic")]
    public async Task<IActionResult> GetMainPageStatistic([FromBody] MainPageStatisticRequest filter)
    {
        var statistic = await _statisticService.GetMainPageStatistic(filter, GetCurrentUserId(), IsAdmin());
        return statistic != null ? Ok(statistic) : StatusCode(500);
    }
}
