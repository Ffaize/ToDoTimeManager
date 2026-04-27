using Dapper;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.DbAccessServices;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Implementation;

public class ProjectTeamsDataController : IProjectTeamsDataController
{
    private readonly IDbAccessService _dbAccessService;
    private readonly ILogger<ProjectTeamsDataController> _logger;

    public ProjectTeamsDataController(IDbAccessService dbAccessService, ILogger<ProjectTeamsDataController> logger)
    {
        _dbAccessService = dbAccessService;
        _logger = logger;
    }

    public async Task<List<ProjectTeamEntity>> GetTeamsByProjectId(Guid projectId)
    {
        try
        {
            return await _dbAccessService.GetAllByParameter<ProjectTeamEntity>("sp_ProjectTeams_GetByProjectId",
                "ProjectId", projectId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<ProjectTeamEntity?> GetByProjectIdAndTeamId(Guid projectId, Guid teamId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("ProjectId", projectId);
            parameters.Add("TeamId", teamId);
            List<ProjectTeamEntity> results = await _dbAccessService.GetRecordsByParameters<ProjectTeamEntity>(
                "sp_ProjectTeams_GetByProjectIdAndTeamId", parameters);
            return results.FirstOrDefault();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> AddTeam(ProjectTeamEntity newProjectTeam)
    {
        try
        {
            return await _dbAccessService.AddRecord("sp_ProjectTeams_Create", newProjectTeam) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> RemoveTeam(Guid projectId, Guid teamId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("ProjectId", projectId);
            parameters.Add("TeamId", teamId);
            return await _dbAccessService.ExecuteByParameters(
                "sp_ProjectTeams_DeleteByProjectIdAndTeamId", parameters) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}