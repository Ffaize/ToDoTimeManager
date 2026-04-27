using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

public interface ITeamsDataController
{
    Task<List<TeamEntity>> GetAllTeams();
    Task<TeamEntity?> GetTeamById(Guid teamId);
    Task<List<TeamEntity>> GetTeamsByUserId(Guid userId);
    Task<bool> CreateTeam(TeamEntity newTeam);
    Task<bool> UpdateTeam(TeamEntity updatedTeam);
    Task<bool> DeleteTeam(Guid teamId);
}