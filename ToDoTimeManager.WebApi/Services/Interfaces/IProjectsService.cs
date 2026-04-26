using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IProjectsService
{
    Task<List<ProjectResponseDto>> GetAllProjects();
    Task<ProjectResponseDto?>      GetProjectById(Guid projectId);
    Task<List<ProjectResponseDto>> GetProjectsByUserId(Guid userId);
    Task<bool>                     CreateProject(CreateProjectRequestDto request, Guid createdByUserId);
    Task<bool>                     UpdateProject(UpdateProjectRequestDto request);
    Task<bool>                     DeleteProject(Guid projectId);
    Task<bool>                     AddTeam(ProjectTeamUpsertRequestDto request);
    Task<bool>                     RemoveTeam(Guid projectId, Guid teamId);
    Task<ProjectTeam?>             GetProjectTeam(Guid projectId, Guid teamId);
    Task<List<ToDo>>               GetToDosByProjectId(Guid projectId);
    Task<bool>                     UserHasAccessToProject(Guid projectId, Guid userId);
}
