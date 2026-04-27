using Dapper;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.DbAccessServices;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Implementation;

public class TeamMembersDataController : ITeamMembersDataController
{
    private readonly IDbAccessService _dbAccessService;
    private readonly ILogger<TeamMembersDataController> _logger;

    public TeamMembersDataController(IDbAccessService dbAccessService, ILogger<TeamMembersDataController> logger)
    {
        _dbAccessService = dbAccessService;
        _logger = logger;
    }

    public async Task<List<TeamMemberEntity>> GetMembersByTeamId(Guid teamId)
    {
        try
        {
            return await _dbAccessService.GetAllByParameter<TeamMemberEntity>("sp_TeamMembers_GetByTeamId", "TeamId",
                teamId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<TeamMemberEntity?> GetMemberByTeamIdAndUserId(Guid teamId, Guid userId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("TeamId", teamId);
            parameters.Add("UserId", userId);
            List<TeamMemberEntity> results = await _dbAccessService.GetRecordsByParameters<TeamMemberEntity>(
                "sp_TeamMembers_GetByTeamIdAndUserId", parameters);
            return results.FirstOrDefault();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> AddMember(TeamMemberEntity newMember)
    {
        try
        {
            return await _dbAccessService.AddRecord("sp_TeamMembers_Create", newMember) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> RemoveMember(Guid teamId, Guid userId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("TeamId", teamId);
            parameters.Add("UserId", userId);
            return await _dbAccessService.ExecuteByParameters(
                "sp_TeamMembers_DeleteByTeamIdAndUserId", parameters) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}