using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IProjectsService
{
    Task<List<ProjectResponseDto>> GetAllProjects();
    Task<ProjectResponseDto?>      GetProjectById(Guid projectId, Guid currentUserId, bool isAdmin);
    Task<List<ProjectResponseDto>> GetProjectsByUserId(Guid userId);
    Task<bool>                     CreateProject(CreateProjectRequestDto request, Guid createdByUserId);
    Task<bool>                     UpdateProject(UpdateProjectRequestDto request, Guid currentUserId, bool isAdmin);
    Task<bool>                     DeleteProject(Guid projectId);
    Task<bool>                     AddTeam(ProjectTeamUpsertRequestDto request, Guid currentUserId, bool isAdmin);
    Task<bool>                     RemoveTeam(Guid projectId, Guid teamId, Guid currentUserId, bool isAdmin);
    Task<List<ToDo>>               GetToDosByProjectId(Guid projectId, Guid currentUserId, bool isAdmin);
}
