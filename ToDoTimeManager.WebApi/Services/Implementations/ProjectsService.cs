using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Exceptions;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class ProjectsService : IProjectsService
{
    private readonly IProjectsDataController     _projectsDataController;
    private readonly IProjectTeamsDataController _projectTeamsDataController;
    private readonly IToDosService               _toDosService;
    private readonly ILogger<ProjectsService>    _logger;

    public ProjectsService(
        IProjectsDataController     projectsDataController,
        IProjectTeamsDataController projectTeamsDataController,
        IToDosService               toDosService,
        ILogger<ProjectsService>    logger)
    {
        _projectsDataController     = projectsDataController;
        _projectTeamsDataController = projectTeamsDataController;
        _toDosService               = toDosService;
        _logger                     = logger;
    }

    public async Task<List<ProjectResponseDto>> GetAllProjects()
    {
        var entities = await _projectsDataController.GetAllProjects();
        return entities.Select(e => MapToDto(e.ToProject(), null)).ToList();
    }

    public async Task<ProjectResponseDto?> GetProjectById(Guid projectId, Guid currentUserId, bool isAdmin)
    {
        if (projectId == Guid.Empty)
            throw new ValidationException("Invalid project ID");

        try
        {
            var entity = await _projectsDataController.GetProjectById(projectId);
            if (entity == null)
                throw new NotFoundException("Project was not found");

            if (!isAdmin && !await UserHasAccess(projectId, currentUserId, entity.CreatedBy))
                throw new ForbiddenException();

            var teamEntities = await _projectTeamsDataController.GetTeamsByProjectId(projectId);
            var teams = teamEntities.Select(t => t.ToProjectTeam()).ToList();
            return MapToDto(entity.ToProject(), teams);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<List<ProjectResponseDto>> GetProjectsByUserId(Guid userId)
    {
        var entities = await _projectsDataController.GetProjectsByUserId(userId);
        return entities.Select(e => MapToDto(e.ToProject(), null)).ToList();
    }

    public async Task<bool> CreateProject(CreateProjectRequestDto request, Guid createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Project name is required");

        try
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
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateProject(UpdateProjectRequestDto request, Guid currentUserId, bool isAdmin)
    {
        if (request.Id == Guid.Empty)
            throw new ValidationException("Invalid project ID");

        try
        {
            if (!isAdmin)
            {
                var project = await _projectsDataController.GetProjectById(request.Id);
                if (project == null)
                    throw new NotFoundException("Project was not found");
                if (project.CreatedBy != currentUserId)
                    throw new ForbiddenException();
            }

            var entity = new ProjectEntity
            {
                Id          = request.Id,
                Name        = request.Name,
                Description = request.Description
            };
            return await _projectsDataController.UpdateProject(entity);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteProject(Guid projectId)
    {
        if (projectId == Guid.Empty)
            throw new ValidationException("Invalid project ID");

        try
        {
            return await _projectsDataController.DeleteProject(projectId);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> AddTeam(ProjectTeamUpsertRequestDto request, Guid currentUserId, bool isAdmin)
    {
        if (request.ProjectId == Guid.Empty)
            throw new ValidationException("Invalid project ID");

        try
        {
            if (!isAdmin)
            {
                var project = await _projectsDataController.GetProjectById(request.ProjectId);
                if (project == null)
                    throw new NotFoundException("Project was not found");
                if (project.CreatedBy != currentUserId)
                    throw new ForbiddenException();
            }

            var existing = await _projectTeamsDataController.GetByProjectIdAndTeamId(request.ProjectId, request.TeamId);
            if (existing != null)
                throw new ConflictException("Team is already linked to this project");

            var entity = new ProjectTeamEntity
            {
                Id        = request.Id,
                ProjectId = request.ProjectId,
                TeamId    = request.TeamId
            };
            return await _projectTeamsDataController.AddTeam(entity);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> RemoveTeam(Guid projectId, Guid teamId, Guid currentUserId, bool isAdmin)
    {
        if (projectId == Guid.Empty || teamId == Guid.Empty)
            throw new ValidationException("Invalid project or team ID");

        try
        {
            if (!isAdmin)
            {
                var project = await _projectsDataController.GetProjectById(projectId);
                if (project == null)
                    throw new NotFoundException("Project was not found");
                if (project.CreatedBy != currentUserId)
                    throw new ForbiddenException();
            }

            return await _projectTeamsDataController.RemoveTeam(projectId, teamId);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<List<ToDo>> GetToDosByProjectId(Guid projectId, Guid currentUserId, bool isAdmin)
    {
        if (projectId == Guid.Empty)
            throw new ValidationException("Invalid project ID");

        try
        {
            if (!isAdmin)
            {
                var project = await _projectsDataController.GetProjectById(projectId);
                if (project == null)
                    throw new NotFoundException("Project was not found");

                if (!await UserHasAccess(projectId, currentUserId, project.CreatedBy))
                    throw new ForbiddenException();
            }

            return await _toDosService.GetToDosByProjectId(projectId);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    private async Task<bool> UserHasAccess(Guid projectId, Guid userId, Guid createdBy)
    {
        if (createdBy == userId) return true;
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
