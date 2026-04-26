using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TimeLogsController : ControllerBase
{
    private readonly ITimeLogsService _timeLogsService;

    public TimeLogsController(ITimeLogsService timeLogsService)
    {
        _timeLogsService = timeLogsService;
    }

    private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsAdmin() => User.IsInRole("Admin");

    [Authorize(Roles = "Admin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllTimeLogs()
    {
        var timeLogs = await _timeLogsService.GetAllTimeLogs();
        return Ok(timeLogs);
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetTimeLogById(Guid id)
    {
        var timeLog = await _timeLogsService.GetTimeLogById(id, GetCurrentUserId(), IsAdmin());
        return timeLog != null ? Ok(timeLog) : StatusCode(500);
    }

    [HttpGet("GetByToDoId/{toDoId}")]
    public async Task<IActionResult> GetTimeLogsByToDoId(Guid toDoId)
    {
        var timeLogs = await _timeLogsService.GetTimeLogsByToDoId(toDoId);
        return Ok(timeLogs);
    }

    [HttpGet("GetByUserId/{userId}")]
    public async Task<IActionResult> GetTimeLogsByUserId(Guid userId)
    {
        var timeLogs = await _timeLogsService.GetTimeLogsByUserId(userId, GetCurrentUserId(), IsAdmin());
        return Ok(timeLogs);
    }

    [HttpGet("GetByUserIdAndToDoId/{userId}/{toDoId}")]
    public async Task<IActionResult> GetTimeLogsByUserIdAndToDoId(Guid userId, Guid toDoId)
    {
        var timeLogs = await _timeLogsService.GetTimeLogsByUserIdAndToDoId(toDoId, userId, GetCurrentUserId(), IsAdmin());
        return Ok(timeLogs);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateTimeLog([FromBody] TimeLogUpsertRequestDto request)
    {
        var timeLog = new TimeLog
        {
            Id             = request.Id,
            ToDoId         = request.ToDoId,
            UserId         = request.UserId,
            HoursSpent     = request.HoursSpent!.Value,
            LogDate        = request.LogDate!.Value,
            LogDescription = request.LogDescription
        };

        var created = await _timeLogsService.CreateTimeLog(timeLog);
        return created ? Ok(created) : StatusCode(500);
    }

    [HttpPut("Update")]
    public async Task<IActionResult> UpdateTimeLog([FromBody] TimeLogUpsertRequestDto request)
    {
        var timeLog = new TimeLog
        {
            Id             = request.Id,
            ToDoId         = request.ToDoId,
            UserId         = request.UserId,
            HoursSpent     = request.HoursSpent!.Value,
            LogDate        = request.LogDate!.Value,
            LogDescription = request.LogDescription
        };

        var updated = await _timeLogsService.UpdateTimeLog(timeLog, GetCurrentUserId(), IsAdmin());
        return updated ? Ok(updated) : StatusCode(500);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteTimeLog(Guid id)
    {
        var deleted = await _timeLogsService.DeleteTimeLog(id, GetCurrentUserId(), IsAdmin());
        return deleted ? Ok(deleted) : StatusCode(500);
    }
}
