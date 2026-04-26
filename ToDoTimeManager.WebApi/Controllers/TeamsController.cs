using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamsService _teamsService;

    public TeamsController(ITeamsService teamsService)
    {
        _teamsService = teamsService;
    }

    private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsAdmin() => User.IsInRole("Admin");

    [Authorize(Roles = "Admin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllTeams()
    {
        var teams = await _teamsService.GetAllTeams();
        return Ok(teams);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteTeam(Guid id)
    {
        var result = await _teamsService.DeleteTeam(id);
        return result ? Ok(result) : StatusCode(500);
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetTeamById(Guid id)
    {
        var team = await _teamsService.GetTeamById(id, GetCurrentUserId(), IsAdmin());
        return team != null ? Ok(team) : StatusCode(500);
    }

    [HttpGet("GetToDosByTeamId/{teamId}")]
    public async Task<IActionResult> GetToDosByTeamId(Guid teamId)
    {
        var todos = await _teamsService.GetToDosByTeamId(teamId, GetCurrentUserId(), IsAdmin());
        return Ok(todos);
    }

    [HttpPut("Update")]
    public async Task<IActionResult> UpdateTeam([FromBody] UpdateTeamRequestDto request)
    {
        var result = await _teamsService.UpdateTeam(request, GetCurrentUserId(), IsAdmin());
        return result ? Ok(result) : StatusCode(500);
    }

    [HttpPost("AddMember")]
    public async Task<IActionResult> AddMember([FromBody] TeamMemberUpsertRequestDto request)
    {
        var result = await _teamsService.AddMember(request, GetCurrentUserId(), IsAdmin());
        return result ? Ok(result) : StatusCode(500);
    }

    [HttpDelete("RemoveMember/{teamId}/{userId}")]
    public async Task<IActionResult> RemoveMember(Guid teamId, Guid userId)
    {
        var result = await _teamsService.RemoveMember(teamId, userId, GetCurrentUserId(), IsAdmin());
        return result ? Ok(result) : StatusCode(500);
    }

    [HttpGet("GetMyTeams")]
    public async Task<IActionResult> GetMyTeams()
    {
        var teams = await _teamsService.GetTeamsByUserId(GetCurrentUserId());
        return Ok(teams);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequestDto request)
    {
        var result = await _teamsService.CreateTeam(request, GetCurrentUserId());
        return result ? Ok(result) : StatusCode(500);
    }
}
