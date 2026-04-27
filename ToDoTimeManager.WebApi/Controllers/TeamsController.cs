using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

/// <summary>
/// Manages teams including creation, membership, to-do retrieval, and deletion.
/// All endpoints require an authenticated user.
/// Operations on other users' teams are restricted to administrators or team creators.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamsService _teamsService;

    /// <summary>
    /// Initializes a new instance of <see cref="TeamsController"/>.
    /// </summary>
    /// <param name="teamsService">The service used to perform team operations.</param>
    public TeamsController(ITeamsService teamsService)
    {
        _teamsService = teamsService;
    }

    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    /// <summary>
    /// Retrieves all teams in the system. Restricted to administrators.
    /// </summary>
    /// <returns>200 OK with a list of all teams.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllTeams()
    {
        List<TeamResponseDto> teams = await _teamsService.GetAllTeams();
        return Ok(teams);
    }

    /// <summary>
    /// Permanently deletes a team by its unique identifier. Restricted to administrators.
    /// </summary>
    /// <param name="id">The unique identifier of the team to delete.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if deletion fails.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteTeam(Guid id)
    {
        var result = await _teamsService.DeleteTeam(id);
        return result ? Ok(result) : StatusCode(500);
    }

    /// <summary>
    /// Retrieves a team by its unique identifier.
    /// Administrators may access any team; regular users may only access teams they are a member of.
    /// </summary>
    /// <param name="id">The unique identifier of the team.</param>
    /// <returns>
    /// 200 OK with the team details on success;
    /// 500 Internal Server Error if the team is not found or the caller lacks access.
    /// </returns>
    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetTeamById(Guid id)
    {
        var team = await _teamsService.GetTeamById(id, GetCurrentUserId(), IsAdmin());
        return team != null ? Ok(team) : StatusCode(500);
    }

    /// <summary>
    /// Retrieves all to-do items assigned to a specific team.
    /// Administrators may access any team; regular users may only access teams they are a member of.
    /// </summary>
    /// <param name="teamId">The unique identifier of the team.</param>
    /// <returns>200 OK with a list of to-do items for the given team.</returns>
    [HttpGet("GetToDosByTeamId/{teamId}")]
    public async Task<IActionResult> GetToDosByTeamId(Guid teamId)
    {
        List<ToDo> todos = await _teamsService.GetToDosByTeamId(teamId, GetCurrentUserId(), IsAdmin());
        return Ok(todos);
    }

    /// <summary>
    /// Updates the name and description of an existing team.
    /// Administrators may update any team; regular users may only update teams they created.
    /// </summary>
    /// <param name="request">The update payload containing the team identifier, new name, and optional description.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if the update fails or the caller lacks access.
    /// </returns>
    [HttpPut("Update")]
    public async Task<IActionResult> UpdateTeam([FromBody] UpdateTeamRequestDto request)
    {
        var result = await _teamsService.UpdateTeam(request, GetCurrentUserId(), IsAdmin());
        return result ? Ok(result) : StatusCode(500);
    }

    /// <summary>
    /// Adds a user as a member of a team with the specified role.
    /// Administrators may modify any team; regular users may only modify teams they created.
    /// </summary>
    /// <param name="request">The payload containing the team identifier, user identifier, and the role to assign.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if the operation fails or the caller lacks access.
    /// </returns>
    [HttpPost("AddMember")]
    public async Task<IActionResult> AddMember([FromBody] TeamMemberUpsertRequestDto request)
    {
        var result = await _teamsService.AddMember(request, GetCurrentUserId(), IsAdmin());
        return result ? Ok(result) : StatusCode(500);
    }

    /// <summary>
    /// Removes a user from a team.
    /// Administrators may modify any team; regular users may only modify teams they created.
    /// </summary>
    /// <param name="teamId">The unique identifier of the team.</param>
    /// <param name="userId">The unique identifier of the user to remove from the team.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if the operation fails or the caller lacks access.
    /// </returns>
    [HttpDelete("RemoveMember/{teamId}/{userId}")]
    public async Task<IActionResult> RemoveMember(Guid teamId, Guid userId)
    {
        var result = await _teamsService.RemoveMember(teamId, userId, GetCurrentUserId(), IsAdmin());
        return result ? Ok(result) : StatusCode(500);
    }

    /// <summary>
    /// Retrieves all teams that the currently authenticated user is a member of.
    /// </summary>
    /// <returns>200 OK with a list of the current user's teams.</returns>
    [HttpGet("GetMyTeams")]
    public async Task<IActionResult> GetMyTeams()
    {
        List<TeamResponseDto> teams = await _teamsService.GetTeamsByUserId(GetCurrentUserId());
        return Ok(teams);
    }

    /// <summary>
    /// Creates a new team owned by the currently authenticated user.
    /// </summary>
    /// <param name="request">The creation payload containing the team name and optional description.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if creation fails.
    /// </returns>
    [HttpPost("Create")]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequestDto request)
    {
        var result = await _teamsService.CreateTeam(request, GetCurrentUserId());
        return result ? Ok(result) : StatusCode(500);
    }
}