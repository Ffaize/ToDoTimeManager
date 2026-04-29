using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

/// <summary>
/// Manages to-do items. All endpoints require an authenticated user.
/// Access to other users' items is restricted to administrators.
/// </summary>
[Authorize]
public class ToDosController : BaseController
{
    private readonly IToDosService _toDosService;

    /// <summary>
    /// Initializes a new instance of <see cref="ToDosController"/>.
    /// </summary>
    /// <param name="toDosService">The service used to perform to-do CRUD operations.</param>
    public ToDosController(IToDosService toDosService)
    {
        _toDosService = toDosService;
    }

    /// <summary>
    /// Retrieves every to-do item in the system regardless of the assigned user.
    /// Restricted to administrators.
    /// </summary>
    /// <returns>200 OK with a list of all <see cref="ToDo"/> items.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllToDos()
    {
        List<ToDo> toDos = await _toDosService.GetAllToDos();
        return Ok(toDos);
    }

    /// <summary>
    /// Retrieves a single to-do item by its unique identifier.
    /// Administrators may access any item; regular users may only access items assigned to them.
    /// </summary>
    /// <param name="id">The unique identifier of the to-do item.</param>
    /// <returns>
    /// 200 OK with the matching <see cref="ToDo"/> on success;
    /// 500 Internal Server Error if the item is not found or the caller lacks access.
    /// </returns>
    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetToDoById(Guid id)
    {
        var toDo = await _toDosService.GetToDoById(id, GetCurrentUserId(), IsAdmin());
        return toDo != null ? Ok(toDo) : StatusCode(500);
    }

    /// <summary>
    /// Retrieves all to-do items assigned to the specified user.
    /// Administrators may query any user; regular users may only query their own items.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose to-dos to retrieve.</param>
    /// <returns>200 OK with a list of <see cref="ToDo"/> items for the given user.</returns>
    [HttpGet("GetByUserId/{userId}")]
    public async Task<IActionResult> GetToDosByUserId(Guid userId)
    {
        List<ToDo> toDos = await _toDosService.GetToDosByUserId(userId, GetCurrentUserId(), IsAdmin());
        return Ok(toDos);
    }

    /// <summary>
    /// Creates a new to-do item from the supplied request payload.
    /// </summary>
    /// <param name="request">
    /// The creation payload containing the title, description, status, optional due date,
    /// and optional assignments to a user, team, and project.
    /// </param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if creation fails.
    /// </returns>
    [HttpPost("Create")]
    public async Task<IActionResult> CreateToDo([FromBody] ToDoUpsertRequestDto request)
    {
        var toDo = new ToDo
        {
            Id = request.Id,
            NumberedId = request.NumberedId,
            Title = request.Title,
            Description = request.Description,
            CreatedAt = request.CreatedAt!.Value,
            DueDate = request.DueDate,
            Status = request.Status!.Value,
            AssignedTo = request.AssignedTo,
            TeamId = request.TeamId,
            ProjectId = request.ProjectId
        };

        var created = await _toDosService.CreateToDo(toDo);
        return created ? Ok(created) : StatusCode(500);
    }

    /// <summary>
    /// Updates an existing to-do item with the values from the supplied request payload.
    /// Administrators may update any item; regular users may only update items assigned to them.
    /// </summary>
    /// <param name="request">
    /// The update payload containing the item's identifier along with the new field values.
    /// </param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if the update fails or the caller lacks access.
    /// </returns>
    [HttpPut("Update")]
    public async Task<IActionResult> UpdateToDo([FromBody] ToDoUpsertRequestDto request)
    {
        var toDo = new ToDo
        {
            Id = request.Id,
            NumberedId = request.NumberedId,
            Title = request.Title,
            Description = request.Description,
            CreatedAt = request.CreatedAt!.Value,
            DueDate = request.DueDate,
            Status = request.Status!.Value,
            AssignedTo = request.AssignedTo,
            TeamId = request.TeamId,
            ProjectId = request.ProjectId
        };

        var updated = await _toDosService.UpdateToDo(toDo, GetCurrentUserId(), IsAdmin());
        return updated ? Ok(updated) : StatusCode(500);
    }

    /// <summary>
    /// Permanently deletes a to-do item by its unique identifier.
    /// Administrators may delete any item; regular users may only delete items assigned to them.
    /// </summary>
    /// <param name="id">The unique identifier of the to-do item to delete.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if deletion fails or the caller lacks access.
    /// </returns>
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteToDo(Guid id)
    {
        var deleted = await _toDosService.DeleteToDo(id, GetCurrentUserId(), IsAdmin());
        return deleted ? Ok(deleted) : StatusCode(500);
    }
}