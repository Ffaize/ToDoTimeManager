using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Extensions;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Exceptions;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class ProjectsService : IProjectsService
{
    private readonly IProjectsDataController _projectsDataController;
    private readonly IProjectTeamsDataController _projectTeamsDataController;
    private readonly IToDosService _toDosService;
    private readonly IAccessControlService _accessControlService;
    private readonly ILogger<ProjectsService> _logger;

    public ProjectsService(
        IProjectsDataController projectsDataController,
        IProjectTeamsDataController projectTeamsDataController,
        IToDosService toDosService,
        IAccessControlService accessControlService,
        ILogger<ProjectsService> logger)
    {
        _projectsDataController = projectsDataController;
        _projectTeamsDataController = projectTeamsDataController;
        _toDosService = toDosService;
        _accessControlService = accessControlService;
        _logger = logger;
    }

    public async Task<List<ProjectResponseDto>> GetAllProjects(Guid currentUserId, UserRole currentUserRole)
    {
        if (currentUserRole >= UserRole.Manager)
        {
            List<ProjectEntity> all = await _projectsDataController.GetAllProjects();
            return all.Select(e => e.ToProject().ToResponseDto(null)).ToList();
        }

        // ProjectManager sees only accessible projects
        List<ProjectEntity> accessible = await _projectsDataController.GetProjectsByUserId(currentUserId);
        return accessible.Select(e => e.ToProject().ToResponseDto(null)).ToList();
    }

    public async Task<ProjectResponseDto?> GetProjectById(Guid projectId, Guid currentUserId, UserRole currentUserRole)
    {
        if (projectId == Guid.Empty)
            throw new ValidationException("Invalid project ID");

        try
        {
            var entity = await _projectsDataController.GetProjectById(projectId);
            if (entity == null)
                throw new NotFoundException("Project was not found");

            if (!await _accessControlService.IsAccessibleToUser(currentUserId, projectId, nameof(GetProjectById)))
                throw new ForbiddenException();

            List<ProjectTeamEntity> teamEntities = await _projectTeamsDataController.GetTeamsByProjectId(projectId);
            List<ProjectTeam> teams = teamEntities.Select(t => t.ToProjectTeam()).ToList();
            return entity.ToProject().ToResponseDto(teams);
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
        List<ProjectEntity> entities = await _projectsDataController.GetProjectsByUserId(userId);
        return entities.Select(e => e.ToProject().ToResponseDto(null)).ToList();
    }

    public async Task<bool> CreateProject(CreateProjectRequestDto request, Guid createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Project name is required");

        try
        {
            var entity = new ProjectEntity
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdByUserId
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

    public async Task<bool> UpdateProject(UpdateProjectRequestDto request, Guid currentUserId, UserRole currentUserRole)
    {
        if (request.Id == Guid.Empty)
            throw new ValidationException("Invalid project ID");

        try
        {
            if (!await _accessControlService.IsAccessibleToUser(currentUserId, request.Id, nameof(UpdateProject)))
                throw new ForbiddenException();

            var entity = new ProjectEntity
            {
                Id = request.Id,
                Name = request.Name,
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

    public async Task<bool> AddTeam(ProjectTeamUpsertRequestDto request, Guid currentUserId, UserRole currentUserRole)
    {
        if (request.ProjectId == Guid.Empty)
            throw new ValidationException("Invalid project ID");

        try
        {
            if (!await _accessControlService.IsAccessibleToUser(currentUserId, request.ProjectId, nameof(AddTeam)))
                throw new ForbiddenException();

            var existing = await _projectTeamsDataController.GetByProjectIdAndTeamId(request.ProjectId, request.TeamId);
            if (existing != null)
                throw new ConflictException("Team is already linked to this project");

            var entity = new ProjectTeamEntity
            {
                Id = request.Id,
                ProjectId = request.ProjectId,
                TeamId = request.TeamId
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

    public async Task<bool> RemoveTeam(Guid projectId, Guid teamId, Guid currentUserId, UserRole currentUserRole)
    {
        if (projectId == Guid.Empty || teamId == Guid.Empty)
            throw new ValidationException("Invalid project or team ID");

        try
        {
            if (!await _accessControlService.IsAccessibleToUser(currentUserId, projectId, nameof(RemoveTeam)))
                throw new ForbiddenException();

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

    public async Task<List<ToDo>> GetToDosByProjectId(Guid projectId, Guid currentUserId, UserRole currentUserRole)
    {
        if (projectId == Guid.Empty)
            throw new ValidationException("Invalid project ID");

        try
        {
            if (!await _accessControlService.IsAccessibleToUser(currentUserId, projectId, nameof(GetToDosByProjectId)))
                throw new ForbiddenException();

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

}
