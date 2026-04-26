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
    private readonly ILogger<ToDosController> _logger;
    private readonly IToDosService _toDosService;

    public ToDosController(ILogger<ToDosController> logger, IToDosService toDosService)
    {
        _logger = logger;
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
        if (id == Guid.Empty)
            return BadRequest("Invalid to-do ID");
        var toDo = await _toDosService.GetToDoById(id);
        if (toDo == null) return NotFound("To-do was not found");
        if (toDo.AssignedTo != GetCurrentUserId() && !IsAdmin())
            return Forbid();
        return Ok(toDo);
    }

    [HttpGet("GetByUserId/{userId}")]
    public async Task<IActionResult> GetToDosByUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");
        if (userId != GetCurrentUserId() && !IsAdmin())
            return Forbid();
        var toDos = await _toDosService.GetToDosByUserId(userId);
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

        var newToDo = await _toDosService.CreateToDo(toDo);
        return newToDo ? Ok(newToDo) : BadRequest("To-do could not be created");
    }

    [HttpPut("Update")]
    public async Task<IActionResult> UpdateToDo([FromBody] ToDoUpsertRequestDto request)
    {
        var existing = await _toDosService.GetToDoById(request.Id);
        if (existing == null) return NotFound("To-do was not found");
        if (existing.AssignedTo != GetCurrentUserId() && !IsAdmin())
            return Forbid();

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

        var updatedToDo = await _toDosService.UpdateToDo(toDo);
        return updatedToDo ? Ok(updatedToDo) : BadRequest("To-do could not be updated");
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteToDo(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid to-do ID");
        var existing = await _toDosService.GetToDoById(id);
        if (existing == null) return NotFound("To-do was not found");
        if (existing.AssignedTo != GetCurrentUserId() && !IsAdmin())
            return Forbid();
        var deletedToDo = await _toDosService.DeleteToDo(id);
        return deletedToDo ? Ok(deletedToDo) : BadRequest("To-do could not be deleted");
    }
}
