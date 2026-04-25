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
    private readonly ILogger<TimeLogsController> _logger;
    private readonly ITimeLogsService _timeLogsService;

    public TimeLogsController(ILogger<TimeLogsController> logger, ITimeLogsService timeLogsService)
    {
        _logger = logger;
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
        if (id == Guid.Empty)
            return BadRequest("Invalid time log ID");
        var timeLog = await _timeLogsService.GetTimeLogById(id);
        if (timeLog == null) return NotFound("Time log was not found");
        if (timeLog.UserId != GetCurrentUserId() && !IsAdmin())
            return Forbid();
        return Ok(timeLog);
    }

    [HttpGet("GetByToDoId/{toDoId}")]
    public async Task<IActionResult> GetTimeLogsByToDoId(Guid toDoId)
    {
        if (toDoId == Guid.Empty)
            return BadRequest("Invalid to-do ID");
        var timeLogs = await _timeLogsService.GetTimeLogsByToDoId(toDoId);
        return Ok(timeLogs);
    }

    [HttpGet("GetByUserId/{userId}")]
    public async Task<IActionResult> GetTimeLogsByUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");
        if (userId != GetCurrentUserId() && !IsAdmin())
            return Forbid();
        var timeLogs = await _timeLogsService.GetTimeLogsByUserId(userId);
        return Ok(timeLogs);
    }

    [HttpGet("GetByUserIdAndToDoId/{userId}/{toDoId}")]
    public async Task<IActionResult> GetTimeLogsByUserIdAndToDoId(Guid userId, Guid toDoId)
    {
        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");
        if (toDoId == Guid.Empty)
            return BadRequest("Invalid to-do ID");
        if (userId != GetCurrentUserId() && !IsAdmin())
            return Forbid();
        var timeLogs = await _timeLogsService.GetTimeLogsByUserIdAndToDoId(toDoId, userId);
        return Ok(timeLogs);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateTimeLog([FromBody] TimeLogUpsertRequestDto request)
    {
        var timeLog = new TimeLog
        {
            Id = request.Id,
            ToDoId = request.ToDoId,
            UserId = request.UserId,
            HoursSpent = request.HoursSpent!.Value,
            LogDate = request.LogDate!.Value,
            LogDescription = request.LogDescription
        };

        var newTimeLog = await _timeLogsService.CreateTimeLog(timeLog);
        return newTimeLog ? Ok(newTimeLog) : BadRequest("Time log could not be created");
    }

    [HttpPut("Update")]
    public async Task<IActionResult> UpdateTimeLog([FromBody] TimeLogUpsertRequestDto request)
    {
        var existing = await _timeLogsService.GetTimeLogById(request.Id);
        if (existing == null) return NotFound("Time log was not found");
        if (existing.UserId != GetCurrentUserId() && !IsAdmin())
            return Forbid();

        var timeLog = new TimeLog
        {
            Id = request.Id,
            ToDoId = request.ToDoId,
            UserId = request.UserId,
            HoursSpent = request.HoursSpent!.Value,
            LogDate = request.LogDate!.Value,
            LogDescription = request.LogDescription
        };

        var updateTimeLog = await _timeLogsService.UpdateTimeLog(timeLog);
        return updateTimeLog ? Ok(updateTimeLog) : BadRequest("Time log could not be updated");
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteTimeLog(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid time log ID");
        var existing = await _timeLogsService.GetTimeLogById(id);
        if (existing == null) return NotFound("Time log was not found");
        if (existing.UserId != GetCurrentUserId() && !IsAdmin())
            return Forbid();
        var deleted = await _timeLogsService.DeleteTimeLog(id);
        return deleted ? Ok(deleted) : BadRequest("Time log could not be deleted");
    }
}
