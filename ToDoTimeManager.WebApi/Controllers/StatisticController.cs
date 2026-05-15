using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.Business.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

/// <summary>
/// Provides aggregated statistics about to-do items and time logs.
/// All endpoints require an authenticated user.
/// Querying another user's statistics is restricted to administrators.
/// </summary>
[Authorize]
public class StatisticController : BaseController
{
    private readonly IStatisticService _statisticService;

    /// <summary>
    /// Initializes a new instance of <see cref="StatisticController"/>.
    /// </summary>
    /// <param name="statisticService">The service used to compute and retrieve statistics.</param>
    public StatisticController(IStatisticService statisticService)
    {
        _statisticService = statisticService;
    }

    /// <summary>
    /// Retrieves the all-time count of to-do items grouped by status for a specific user.
    /// Administrators may query any user; regular users may only query their own statistics.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose statistics to retrieve.</param>
    /// <returns>
    /// 200 OK with a list of <see cref="ToDoCountStatisticsOfAllTime"/> entries,
    /// one per distinct to-do status.
    /// </returns>
    [HttpGet("GetToDoCountStatisticsOfAllTimeByUserId/{userId}")]
    public async Task<IActionResult> GetToDoCountStatisticsOfAllTimeByUserId(Guid userId)
    {
        List<ToDoCountStatisticsOfAllTime> statistics =
            await _statisticService.GetToDoCountStatisticsOfAllTimeByUserId(userId, GetCurrentUserId(), GetCurrentUserRole());
        return Ok(statistics);
    }

    /// <summary>
    /// Retrieves the main dashboard statistics for a user, including time logs for a selected period,
    /// time logs for the current month, upcoming due-date tasks, and all-time to-do status counts.
    /// Administrators may query any user; regular users may only query their own statistics.
    /// </summary>
    /// <param name="filter">
    /// The request payload specifying the target user identifier and the
    /// <see cref="TimeFilter"/> that determines the time range for the period statistics.
    /// </param>
    /// <returns>
    /// 200 OK with a <see cref="MainPageStatisticModel"/> on success;
    /// 500 Internal Server Error if the query fails or the caller lacks access.
    /// </returns>
    [HttpPost("GetMainPageStatistic")]
    public async Task<IActionResult> GetMainPageStatistic([FromBody] MainPageStatisticRequestDto filter)
    {
        var statistic = await _statisticService.GetMainPageStatistic(filter, GetCurrentUserId(), GetCurrentUserRole());
        return statistic != null ? Ok(statistic) : StatusCode(500);
    }
}