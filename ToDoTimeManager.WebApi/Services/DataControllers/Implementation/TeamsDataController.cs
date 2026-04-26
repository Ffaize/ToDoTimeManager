using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.DbAccessServices;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Implementation;

public class TeamsDataController : ITeamsDataController
{
    private readonly IDbAccessService _dbAccessService;
    private readonly ILogger<TeamsDataController> _logger;

    public TeamsDataController(IDbAccessService dbAccessService, ILogger<TeamsDataController> logger)
    {
        _dbAccessService = dbAccessService;
        _logger = logger;
    }

    public async Task<List<TeamEntity>> GetAllTeams()
    {
        try { return await _dbAccessService.GetAllRecords<TeamEntity>("sp_Teams_GetAll"); }
        catch (Exception e) { _logger.LogError(e, e.Message); return []; }
    }

    public async Task<TeamEntity?> GetTeamById(Guid teamId)
    {
        try { return await _dbAccessService.GetRecordById<TeamEntity>("sp_Teams_GetById", teamId); }
        catch (Exception e) { _logger.LogError(e, e.Message); return null; }
    }

    public async Task<List<TeamEntity>> GetTeamsByUserId(Guid userId)
    {
        try { return await _dbAccessService.GetAllByParameter<TeamEntity>("sp_Teams_GetByUserId", "UserId", userId); }
        catch (Exception e) { _logger.LogError(e, e.Message); return []; }
    }

    public async Task<bool> CreateTeam(TeamEntity newTeam)
    {
        try { return await _dbAccessService.AddRecord("sp_Teams_Create", newTeam) >= 1; }
        catch (Exception e) { _logger.LogError(e, e.Message); return false; }
    }

    public async Task<bool> UpdateTeam(TeamEntity updatedTeam)
    {
        try { return await _dbAccessService.UpdateRecord("sp_Teams_Update", updatedTeam) >= 1; }
        catch (Exception e) { _logger.LogError(e, e.Message); return false; }
    }

    public async Task<bool> DeleteTeam(Guid teamId)
    {
        try { return await _dbAccessService.DeleteRecordById("sp_Teams_DeleteById", teamId) >= 1; }
        catch (Exception e) { _logger.LogError(e, e.Message); return false; }
    }
}
