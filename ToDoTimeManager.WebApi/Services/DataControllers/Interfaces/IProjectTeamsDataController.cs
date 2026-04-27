using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

public interface IProjectTeamsDataController
{
    Task<List<ProjectTeamEntity>> GetTeamsByProjectId(Guid projectId);
    Task<ProjectTeamEntity?> GetByProjectIdAndTeamId(Guid projectId, Guid teamId);
    Task<bool> AddTeam(ProjectTeamEntity newProjectTeam);
    Task<bool> RemoveTeam(Guid projectId, Guid teamId);
}