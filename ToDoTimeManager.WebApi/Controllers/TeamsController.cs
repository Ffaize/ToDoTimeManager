using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamsService _teamsService;
    private readonly ILogger<TeamsController> _logger;

    public TeamsController(ITeamsService teamsService, ILogger<TeamsController> logger)
    {
        _teamsService = teamsService;
        _logger = logger;
    }

    private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsAdmin() => User.IsInRole("Admin");

    // ── Admin only ──────────────────────────────────────────────────────────

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
        if (id == Guid.Empty) return BadRequest("Invalid team ID");
        var result = await _teamsService.DeleteTeam(id);
        return result ? Ok(result) : BadRequest("Team could not be deleted");
    }

    // ── Admin or team member ────────────────────────────────────────────────

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetTeamById(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("Invalid team ID");

        var team = await _teamsService.GetTeamById(id);
        if (team == null) return NotFound("Team was not found");

        if (!IsAdmin())
        {
            var membership = await _teamsService.GetMembership(id, GetCurrentUserId());
            if (membership == null) return Forbid();
        }

        return Ok(team);
    }

    [HttpGet("GetToDosByTeamId/{teamId}")]
    public async Task<IActionResult> GetToDosByTeamId(Guid teamId)
    {
        if (teamId == Guid.Empty) return BadRequest("Invalid team ID");

        if (!IsAdmin())
        {
            var membership = await _teamsService.GetMembership(teamId, GetCurrentUserId());
            if (membership == null) return Forbid();
        }

        var todos = await _teamsService.GetToDosByTeamId(teamId);
        return Ok(todos);
    }

    // ── Admin or team owner ─────────────────────────────────────────────────

    [HttpPut("Update")]
    public async Task<IActionResult> UpdateTeam([FromBody] UpdateTeamRequestDto request)
    {
        if (!IsAdmin())
        {
            var membership = await _teamsService.GetMembership(request.Id, GetCurrentUserId());
            if (membership == null || membership.Role != TeamMemberRole.Owner) return Forbid();
        }

        var result = await _teamsService.UpdateTeam(request);
        return result ? Ok(result) : BadRequest("Team could not be updated");
    }

    [HttpPost("AddMember")]
    public async Task<IActionResult> AddMember([FromBody] TeamMemberUpsertRequestDto request)
    {
        if (!IsAdmin())
        {
            var membership = await _teamsService.GetMembership(request.TeamId, GetCurrentUserId());
            if (membership == null || membership.Role != TeamMemberRole.Owner) return Forbid();
        }

        var result = await _teamsService.AddMember(request);
        return result ? Ok(result) : BadRequest("Member could not be added (already exists or invalid data)");
    }

    [HttpDelete("RemoveMember/{teamId}/{userId}")]
    public async Task<IActionResult> RemoveMember(Guid teamId, Guid userId)
    {
        if (teamId == Guid.Empty || userId == Guid.Empty)
            return BadRequest("Invalid team or user ID");

        if (!IsAdmin())
        {
            var membership = await _teamsService.GetMembership(teamId, GetCurrentUserId());
            if (membership == null || membership.Role != TeamMemberRole.Owner) return Forbid();
        }

        var result = await _teamsService.RemoveMember(teamId, userId);
        return result ? Ok(result) : BadRequest("Member could not be removed (last owner or not found)");
    }

    // ── Any authenticated user ──────────────────────────────────────────────

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
        return result ? Ok(result) : BadRequest("Team could not be created");
    }
}
