using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectsService _projectsService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IProjectsService projectsService, ILogger<ProjectsController> logger)
    {
        _projectsService = projectsService;
        _logger = logger;
    }

    private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsAdmin() => User.IsInRole("Admin");

    // ── Admin only ──────────────────────────────────────────────────────────

    [Authorize(Roles = "Admin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllProjects()
    {
        var projects = await _projectsService.GetAllProjects();
        return Ok(projects);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("Invalid project ID");
        var result = await _projectsService.DeleteProject(id);
        return result ? Ok(result) : BadRequest("Project could not be deleted");
    }

    // ── Admin or project-accessible user (read) ─────────────────────────────

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("Invalid project ID");

        var project = await _projectsService.GetProjectById(id);
        if (project == null) return NotFound("Project was not found");

        if (!IsAdmin() && !await _projectsService.UserHasAccessToProject(id, GetCurrentUserId()))
            return Forbid();

        return Ok(project);
    }

    [HttpGet("GetToDosByProjectId/{projectId}")]
    public async Task<IActionResult> GetToDosByProjectId(Guid projectId)
    {
        if (projectId == Guid.Empty) return BadRequest("Invalid project ID");

        if (!IsAdmin() && !await _projectsService.UserHasAccessToProject(projectId, GetCurrentUserId()))
            return Forbid();

        var todos = await _projectsService.GetToDosByProjectId(projectId);
        return Ok(todos);
    }

    // ── Admin or project owner (writes) ────────────────────────────────────

    [HttpPut("Update")]
    public async Task<IActionResult> UpdateProject([FromBody] UpdateProjectRequestDto request)
    {
        if (!IsAdmin())
        {
            var project = await _projectsService.GetProjectById(request.Id);
            if (project == null) return NotFound("Project was not found");
            if (project.CreatedBy != GetCurrentUserId()) return Forbid();
        }

        var result = await _projectsService.UpdateProject(request);
        return result ? Ok(result) : BadRequest("Project could not be updated");
    }

    [HttpPost("AddTeam")]
    public async Task<IActionResult> AddTeam([FromBody] ProjectTeamUpsertRequestDto request)
    {
        if (!IsAdmin())
        {
            var project = await _projectsService.GetProjectById(request.ProjectId);
            if (project == null) return NotFound("Project was not found");
            if (project.CreatedBy != GetCurrentUserId()) return Forbid();
        }

        var result = await _projectsService.AddTeam(request);
        return result ? Ok(result) : BadRequest("Team could not be added (already linked or invalid data)");
    }

    [HttpDelete("RemoveTeam/{projectId}/{teamId}")]
    public async Task<IActionResult> RemoveTeam(Guid projectId, Guid teamId)
    {
        if (projectId == Guid.Empty || teamId == Guid.Empty)
            return BadRequest("Invalid project or team ID");

        if (!IsAdmin())
        {
            var project = await _projectsService.GetProjectById(projectId);
            if (project == null) return NotFound("Project was not found");
            if (project.CreatedBy != GetCurrentUserId()) return Forbid();
        }

        var result = await _projectsService.RemoveTeam(projectId, teamId);
        return result ? Ok(result) : BadRequest("Team could not be removed");
    }

    // ── Any authenticated user ──────────────────────────────────────────────

    [HttpGet("GetMyProjects")]
    public async Task<IActionResult> GetMyProjects()
    {
        var projects = await _projectsService.GetProjectsByUserId(GetCurrentUserId());
        return Ok(projects);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequestDto request)
    {
        var result = await _projectsService.CreateProject(request, GetCurrentUserId());
        return result ? Ok(result) : BadRequest("Project could not be created");
    }
}
