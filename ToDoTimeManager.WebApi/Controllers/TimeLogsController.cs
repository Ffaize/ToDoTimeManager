using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

/// <summary>
/// Manages time log entries that track hours spent on to-do items.
/// All endpoints require an authenticated user.
/// Access to other users' entries is restricted to administrators.
/// </summary>
[Authorize]
public class TimeLogsController : BaseController
{
    private readonly ITimeLogsService _timeLogsService;

    /// <summary>
    /// Initializes a new instance of <see cref="TimeLogsController"/>.
    /// </summary>
    /// <param name="timeLogsService">The service used to perform time-log CRUD operations.</param>
    public TimeLogsController(ITimeLogsService timeLogsService)
    {
        _timeLogsService = timeLogsService;
    }

    /// <summary>
    /// Retrieves every time log entry in the system across all users and to-do items.
    /// Restricted to administrators.
    /// </summary>
    /// <returns>200 OK with a list of all <see cref="TimeLog"/> entries.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllTimeLogs()
    {
        List<TimeLog> timeLogs = await _timeLogsService.GetAllTimeLogs();
        return Ok(timeLogs);
    }

    /// <summary>
    /// Retrieves a single time log entry by its unique identifier.
    /// Administrators may access any entry; regular users may only access their own entries.
    /// </summary>
    /// <param name="id">The unique identifier of the time log entry.</param>
    /// <returns>
    /// 200 OK with the matching <see cref="TimeLog"/> on success;
    /// 500 Internal Server Error if the entry is not found or the caller lacks access.
    /// </returns>
    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetTimeLogById(Guid id)
    {
        var timeLog = await _timeLogsService.GetTimeLogById(id, GetCurrentUserId(), GetCurrentUserRole());
        return timeLog != null ? Ok(timeLog) : StatusCode(500);
    }

    /// <summary>
    /// Retrieves all time log entries associated with a specific to-do item.
    /// </summary>
    /// <param name="toDoId">The unique identifier of the to-do item.</param>
    /// <returns>200 OK with a list of <see cref="TimeLog"/> entries for the given to-do.</returns>
    [HttpGet("GetByToDoId/{toDoId}")]
    public async Task<IActionResult> GetTimeLogsByToDoId(Guid toDoId)
    {
        List<TimeLog> timeLogs = await _timeLogsService.GetTimeLogsByToDoId(toDoId);
        return Ok(timeLogs);
    }

    /// <summary>
    /// Retrieves all time log entries recorded by a specific user.
    /// Administrators may query any user; regular users may only query their own entries.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose time logs to retrieve.</param>
    /// <returns>200 OK with a list of <see cref="TimeLog"/> entries for the given user.</returns>
    [HttpGet("GetByUserId/{userId}")]
    public async Task<IActionResult> GetTimeLogsByUserId(Guid userId)
    {
        List<TimeLog> timeLogs = await _timeLogsService.GetTimeLogsByUserId(userId, GetCurrentUserId(), GetCurrentUserRole());
        return Ok(timeLogs);
    }

    /// <summary>
    /// Retrieves all time log entries recorded by a specific user on a specific to-do item.
    /// Administrators may query any combination; regular users may only query their own entries.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="toDoId">The unique identifier of the to-do item.</param>
    /// <returns>200 OK with a list of matching <see cref="TimeLog"/> entries.</returns>
    [HttpGet("GetByUserIdAndToDoId/{userId}/{toDoId}")]
    public async Task<IActionResult> GetTimeLogsByUserIdAndToDoId(Guid userId, Guid toDoId)
    {
        List<TimeLog> timeLogs =
            await _timeLogsService.GetTimeLogsByUserIdAndToDoId(toDoId, userId, GetCurrentUserId(), GetCurrentUserRole());
        return Ok(timeLogs);
    }

    /// <summary>
    /// Creates a new time log entry recording hours spent on a to-do item.
    /// </summary>
    /// <param name="request">
    /// The creation payload containing the associated to-do ID, user ID,
    /// hours spent, log date, and an optional description.
    /// </param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if creation fails.
    /// </returns>
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

        var created = await _timeLogsService.CreateTimeLog(timeLog);
        return created ? Ok(created) : StatusCode(500);
    }

    /// <summary>
    /// Updates an existing time log entry with the values from the supplied request payload.
    /// Administrators may update any entry; regular users may only update their own entries.
    /// </summary>
    /// <param name="request">
    /// The update payload containing the entry's identifier along with the new field values.
    /// </param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if the update fails or the caller lacks access.
    /// </returns>
    [HttpPut("Update")]
    public async Task<IActionResult> UpdateTimeLog([FromBody] TimeLogUpsertRequestDto request)
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

        var updated = await _timeLogsService.UpdateTimeLog(timeLog, GetCurrentUserId(), GetCurrentUserRole());
        return updated ? Ok(updated) : StatusCode(500);
    }

    /// <summary>
    /// Permanently deletes a time log entry by its unique identifier.
    /// Administrators may delete any entry; regular users may only delete their own entries.
    /// </summary>
    /// <param name="id">The unique identifier of the time log entry to delete.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if deletion fails or the caller lacks access.
    /// </returns>
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteTimeLog(Guid id)
    {
        var deleted = await _timeLogsService.DeleteTimeLog(id, GetCurrentUserId(), GetCurrentUserRole());
        return deleted ? Ok(deleted) : StatusCode(500);
    }
}