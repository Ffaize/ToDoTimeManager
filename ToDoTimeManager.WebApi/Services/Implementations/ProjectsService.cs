using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class ProjectsService : IProjectsService
{
    private readonly IProjectsDataController     _projectsDataController;
    private readonly IProjectTeamsDataController _projectTeamsDataController;
    private readonly IToDosDataController        _toDosDataController;
    private readonly ILogger<ProjectsService>    _logger;

    public ProjectsService(
        IProjectsDataController     projectsDataController,
        IProjectTeamsDataController projectTeamsDataController,
        IToDosDataController        toDosDataController,
        ILogger<ProjectsService>    logger)
    {
        _projectsDataController     = projectsDataController;
        _projectTeamsDataController = projectTeamsDataController;
        _toDosDataController        = toDosDataController;
        _logger                     = logger;
    }

    public async Task<List<ProjectResponseDto>> GetAllProjects()
    {
        var entities = await _projectsDataController.GetAllProjects();
        return entities.Select(e => MapToDto(e.ToProject(), null)).ToList();
    }

    public async Task<ProjectResponseDto?> GetProjectById(Guid projectId)
    {
        var entity = await _projectsDataController.GetProjectById(projectId);
        if (entity == null) return null;

        var teamEntities = await _projectTeamsDataController.GetTeamsByProjectId(projectId);
        var teams = teamEntities.Select(t => t.ToProjectTeam()).ToList();
        return MapToDto(entity.ToProject(), teams);
    }

    public async Task<List<ProjectResponseDto>> GetProjectsByUserId(Guid userId)
    {
        var entities = await _projectsDataController.GetProjectsByUserId(userId);
        return entities.Select(e => MapToDto(e.ToProject(), null)).ToList();
    }

    public async Task<bool> CreateProject(CreateProjectRequestDto request, Guid createdByUserId)
    {
        var entity = new ProjectEntity
        {
            Id          = request.Id,
            Name        = request.Name,
            Description = request.Description,
            CreatedAt   = DateTime.UtcNow,
            CreatedBy   = createdByUserId
        };
        return await _projectsDataController.CreateProject(entity);
    }

    public async Task<bool> UpdateProject(UpdateProjectRequestDto request)
    {
        var entity = new ProjectEntity
        {
            Id          = request.Id,
            Name        = request.Name,
            Description = request.Description
        };
        return await _projectsDataController.UpdateProject(entity);
    }

    public async Task<bool> DeleteProject(Guid projectId)
        => await _projectsDataController.DeleteProject(projectId);

    public async Task<bool> AddTeam(ProjectTeamUpsertRequestDto request)
    {
        var existing = await _projectTeamsDataController.GetByProjectIdAndTeamId(request.ProjectId, request.TeamId);
        if (existing != null) return false;

        var entity = new ProjectTeamEntity
        {
            Id        = request.Id,
            ProjectId = request.ProjectId,
            TeamId    = request.TeamId
        };
        return await _projectTeamsDataController.AddTeam(entity);
    }

    public async Task<bool> RemoveTeam(Guid projectId, Guid teamId)
        => await _projectTeamsDataController.RemoveTeam(projectId, teamId);

    public async Task<ProjectTeam?> GetProjectTeam(Guid projectId, Guid teamId)
    {
        var entity = await _projectTeamsDataController.GetByProjectIdAndTeamId(projectId, teamId);
        return entity?.ToProjectTeam();
    }

    public async Task<List<ToDo>> GetToDosByProjectId(Guid projectId)
    {
        var entities = await _toDosDataController.GetToDosByProjectId(projectId);
        return entities.Select(e => e.ToToDo()).ToList();
    }

    public async Task<bool> UserHasAccessToProject(Guid projectId, Guid userId)
    {
        var project = await _projectsDataController.GetProjectById(projectId);
        if (project == null) return false;
        if (project.CreatedBy == userId) return true;

        var accessible = await _projectsDataController.GetProjectsByUserId(userId);
        return accessible.Any(p => p.Id == projectId);
    }

    private static ProjectResponseDto MapToDto(Project project, List<ProjectTeam>? teams) => new()
    {
        Id          = project.Id,
        Name        = project.Name,
        Description = project.Description,
        CreatedAt   = project.CreatedAt,
        CreatedBy   = project.CreatedBy,
        TeamCount   = project.TeamCount,
        Teams       = teams
    };
}
