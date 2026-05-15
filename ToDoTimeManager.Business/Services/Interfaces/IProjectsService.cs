using ToDoTimeManager.Shared.DTOs.Project;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Interfaces;

public interface IProjectsService
{
    Task<List<ProjectResponseDto>> GetAllProjects(Guid currentUserId, UserRole currentUserRole);
    Task<ProjectResponseDto?> GetProjectById(Guid projectId, Guid currentUserId, UserRole currentUserRole);
    Task<List<ProjectResponseDto>> GetProjectsByUserId(Guid userId);
    Task<bool> CreateProject(CreateProjectRequestDto request, Guid createdByUserId);
    Task<bool> UpdateProject(UpdateProjectRequestDto request, Guid currentUserId, UserRole currentUserRole);
    Task<bool> DeleteProject(Guid projectId);
    Task<bool> AddTeam(ProjectTeamUpsertRequestDto request, Guid currentUserId, UserRole currentUserRole);
    Task<bool> RemoveTeam(Guid projectId, Guid teamId, Guid currentUserId, UserRole currentUserRole);
    Task<List<ToDo>> GetToDosByProjectId(Guid projectId, Guid currentUserId, UserRole currentUserRole);
}
