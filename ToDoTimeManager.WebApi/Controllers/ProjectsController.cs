using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

/// <summary>
/// Manages projects including creation, membership, to-do retrieval, and deletion.
/// All endpoints require an authenticated user.
/// Operations on other users' projects are restricted to administrators or project creators.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectsService _projectsService;

    /// <summary>
    /// Initializes a new instance of <see cref="ProjectsController"/>.
    /// </summary>
    /// <param name="projectsService">The service used to perform project operations.</param>
    public ProjectsController(IProjectsService projectsService)
    {
        _projectsService = projectsService;
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
    /// Retrieves all projects in the system. Restricted to administrators.
    /// </summary>
    /// <returns>200 OK with a list of all projects.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllProjects()
    {
        List<ProjectResponseDto> projects = await _projectsService.GetAllProjects();
        return Ok(projects);
    }

    /// <summary>
    /// Permanently deletes a project by its unique identifier. Restricted to administrators.
    /// </summary>
    /// <param name="id">The unique identifier of the project to delete.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if deletion fails.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var result = await _projectsService.DeleteProject(id);
        return result ? Ok(result) : StatusCode(500);
    }

    /// <summary>
    /// Retrieves a project by its unique identifier.
    /// Administrators may access any project; regular users may only access projects
    /// they are a member of through an assigned team.
    /// </summary>
    /// <param name="id">The unique identifier of the project.</param>
    /// <returns>
    /// 200 OK with the project details on success;
    /// 500 Internal Server Error if the project is not found or the caller lacks access.
    /// </returns>
    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        var project = await _projectsService.GetProjectById(id, GetCurrentUserId(), IsAdmin());
        return project != null ? Ok(project) : StatusCode(500);
    }

    /// <summary>
    /// Retrieves all to-do items that belong to a specific project.
    /// Administrators may access any project; regular users may only access projects
    /// they are a member of through an assigned team.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <returns>200 OK with a list of to-do items for the given project.</returns>
    [HttpGet("GetToDosByProjectId/{projectId}")]
    public async Task<IActionResult> GetToDosByProjectId(Guid projectId)
    {
        List<ToDo> todos = await _projectsService.GetToDosByProjectId(projectId, GetCurrentUserId(), IsAdmin());
        return Ok(todos);
    }

    /// <summary>
    /// Updates the name and description of an existing project.
    /// Administrators may update any project; regular users may only update projects they created.
    /// </summary>
    /// <param name="request">The update payload containing the project identifier, new name, and optional description.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if the update fails or the caller lacks access.
    /// </returns>
    [HttpPut("Update")]
    public async Task<IActionResult> UpdateProject([FromBody] UpdateProjectRequestDto request)
    {
        var result = await _projectsService.UpdateProject(request, GetCurrentUserId(), IsAdmin());
        return result ? Ok(result) : StatusCode(500);
    }

    /// <summary>
    /// Associates a team with a project, granting team members access to the project.
    /// Administrators may modify any project; regular users may only modify projects they created.
    /// </summary>
    /// <param name="request">The payload containing the project identifier and team identifier to associate.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if the operation fails or the caller lacks access.
    /// </returns>
    [HttpPost("AddTeam")]
    public async Task<IActionResult> AddTeam([FromBody] ProjectTeamUpsertRequestDto request)
    {
        var result = await _projectsService.AddTeam(request, GetCurrentUserId(), IsAdmin());
        return result ? Ok(result) : StatusCode(500);
    }

    /// <summary>
    /// Removes a team's association from a project, revoking that team's access.
    /// Administrators may modify any project; regular users may only modify projects they created.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="teamId">The unique identifier of the team to disassociate from the project.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if the operation fails or the caller lacks access.
    /// </returns>
    [HttpDelete("RemoveTeam/{projectId}/{teamId}")]
    public async Task<IActionResult> RemoveTeam(Guid projectId, Guid teamId)
    {
        var result = await _projectsService.RemoveTeam(projectId, teamId, GetCurrentUserId(), IsAdmin());
        return result ? Ok(result) : StatusCode(500);
    }

    /// <summary>
    /// Retrieves all projects that the currently authenticated user has access to via team membership.
    /// </summary>
    /// <returns>200 OK with a list of the current user's accessible projects.</returns>
    [HttpGet("GetMyProjects")]
    public async Task<IActionResult> GetMyProjects()
    {
        List<ProjectResponseDto> projects = await _projectsService.GetProjectsByUserId(GetCurrentUserId());
        return Ok(projects);
    }

    /// <summary>
    /// Creates a new project owned by the currently authenticated user.
    /// </summary>
    /// <param name="request">The creation payload containing the project name and optional description.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if creation fails.
    /// </returns>
    [HttpPost("Create")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequestDto request)
    {
        var result = await _projectsService.CreateProject(request, GetCurrentUserId());
        return result ? Ok(result) : StatusCode(500);
    }
}