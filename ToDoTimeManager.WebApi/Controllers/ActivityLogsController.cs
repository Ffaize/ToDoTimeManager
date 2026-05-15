using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.Business.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

[Authorize]
public class ActivityLogsController : BaseController
{
    private readonly IActivityLogsService _activityLogsService;

    public ActivityLogsController(IActivityLogsService activityLogsService)
    {
        _activityLogsService = activityLogsService;
    }

    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllActivityLogs()
    {
        List<ActivityLog> logs = await _activityLogsService.GetAllActivityLogs();
        return Ok(logs);
    }

    [HttpGet("GetByToDoId/{toDoId}")]
    public async Task<IActionResult> GetActivityLogsByToDoId(Guid toDoId)
    {
        List<ActivityLog> logs = await _activityLogsService.GetActivityLogsByToDoId(
            toDoId, GetCurrentUserId(), GetCurrentUserRole());
        return Ok(logs);
    }

    [HttpGet("GetByUserId/{userId}")]
    public async Task<IActionResult> GetActivityLogsByUserId(Guid userId)
    {
        List<ActivityLog> logs = await _activityLogsService.GetActivityLogsByUserId(
            userId, GetCurrentUserId(), GetCurrentUserRole());
        return Ok(logs);
    }

    [HttpGet("GetByUserIdAndToDoId/{userId}/{toDoId}")]
    public async Task<IActionResult> GetActivityLogsByUserIdAndToDoId(Guid userId, Guid toDoId)
    {
        List<ActivityLog> logs = await _activityLogsService.GetActivityLogsByUserIdAndToDoId(
            toDoId, userId, GetCurrentUserId(), GetCurrentUserRole());
        return Ok(logs);
    }
}
