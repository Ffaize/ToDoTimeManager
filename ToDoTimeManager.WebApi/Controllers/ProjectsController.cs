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

    public ProjectsController(IProjectsService projectsService)
    {
        _projectsService = projectsService;
    }

    private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsAdmin() => User.IsInRole("Admin");

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
        var result = await _projectsService.DeleteProject(id);
        return result ? Ok(result) : StatusCode(500);
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        var project = await _projectsService.GetProjectById(id, GetCurrentUserId(), IsAdmin());
        return project != null ? Ok(project) : StatusCode(500);
    }

    [HttpGet("GetToDosByProjectId/{projectId}")]
    public async Task<IActionResult> GetToDosByProjectId(Guid projectId)
    {
        var todos = await _projectsService.GetToDosByProjectId(projectId, GetCurrentUserId(), IsAdmin());
        return Ok(todos);
    }

    [HttpPut("Update")]
    public async Task<IActionResult> UpdateProject([FromBody] UpdateProjectRequestDto request)
    {
        var result = await _projectsService.UpdateProject(request, GetCurrentUserId(), IsAdmin());
        return result ? Ok(result) : StatusCode(500);
    }

    [HttpPost("AddTeam")]
    public async Task<IActionResult> AddTeam([FromBody] ProjectTeamUpsertRequestDto request)
    {
        var result = await _projectsService.AddTeam(request, GetCurrentUserId(), IsAdmin());
        return result ? Ok(result) : StatusCode(500);
    }

    [HttpDelete("RemoveTeam/{projectId}/{teamId}")]
    public async Task<IActionResult> RemoveTeam(Guid projectId, Guid teamId)
    {
        var result = await _projectsService.RemoveTeam(projectId, teamId, GetCurrentUserId(), IsAdmin());
        return result ? Ok(result) : StatusCode(500);
    }

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
        return result ? Ok(result) : StatusCode(500);
    }
}
