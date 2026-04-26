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
public class ToDosController : ControllerBase
{
    private readonly IToDosService _toDosService;

    public ToDosController(IToDosService toDosService)
    {
        _toDosService = toDosService;
    }

    private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsAdmin() => User.IsInRole("Admin");

    [Authorize(Roles = "Admin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllToDos()
    {
        var toDos = await _toDosService.GetAllToDos();
        return Ok(toDos);
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetToDoById(Guid id)
    {
        var toDo = await _toDosService.GetToDoById(id, GetCurrentUserId(), IsAdmin());
        return toDo != null ? Ok(toDo) : StatusCode(500);
    }

    [HttpGet("GetByUserId/{userId}")]
    public async Task<IActionResult> GetToDosByUserId(Guid userId)
    {
        var toDos = await _toDosService.GetToDosByUserId(userId, GetCurrentUserId(), IsAdmin());
        return Ok(toDos);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateToDo([FromBody] ToDoUpsertRequestDto request)
    {
        var toDo = new ToDo
        {
            Id          = request.Id,
            NumberedId  = request.NumberedId,
            Title       = request.Title,
            Description = request.Description,
            CreatedAt   = request.CreatedAt!.Value,
            DueDate     = request.DueDate,
            Status      = request.Status!.Value,
            AssignedTo  = request.AssignedTo,
            TeamId      = request.TeamId,
            ProjectId   = request.ProjectId
        };

        var created = await _toDosService.CreateToDo(toDo);
        return created ? Ok(created) : StatusCode(500);
    }

    [HttpPut("Update")]
    public async Task<IActionResult> UpdateToDo([FromBody] ToDoUpsertRequestDto request)
    {
        var toDo = new ToDo
        {
            Id          = request.Id,
            NumberedId  = request.NumberedId,
            Title       = request.Title,
            Description = request.Description,
            CreatedAt   = request.CreatedAt!.Value,
            DueDate     = request.DueDate,
            Status      = request.Status!.Value,
            AssignedTo  = request.AssignedTo,
            TeamId      = request.TeamId,
            ProjectId   = request.ProjectId
        };

        var updated = await _toDosService.UpdateToDo(toDo, GetCurrentUserId(), IsAdmin());
        return updated ? Ok(updated) : StatusCode(500);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteToDo(Guid id)
    {
        var deleted = await _toDosService.DeleteToDo(id, GetCurrentUserId(), IsAdmin());
        return deleted ? Ok(deleted) : StatusCode(500);
    }
}
