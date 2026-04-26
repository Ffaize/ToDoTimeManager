using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

public interface ITeamMembersDataController
{
    Task<List<TeamMemberEntity>> GetMembersByTeamId(Guid teamId);
    Task<TeamMemberEntity?>      GetMemberByTeamIdAndUserId(Guid teamId, Guid userId);
    Task<bool>                   AddMember(TeamMemberEntity newMember);
    Task<bool>                   RemoveMember(Guid teamId, Guid userId);
}
