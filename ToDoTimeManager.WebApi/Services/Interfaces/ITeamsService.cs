using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface ITeamsService
{
    Task<List<TeamResponseDto>> GetAllTeams();
    Task<TeamResponseDto?>      GetTeamById(Guid teamId);
    Task<List<TeamResponseDto>> GetTeamsByUserId(Guid userId);
    Task<bool>                  CreateTeam(CreateTeamRequestDto request, Guid createdByUserId);
    Task<bool>                  UpdateTeam(UpdateTeamRequestDto request);
    Task<bool>                  DeleteTeam(Guid teamId);
    Task<bool>                  AddMember(TeamMemberUpsertRequestDto request);
    Task<bool>                  RemoveMember(Guid teamId, Guid userId);
    Task<TeamMember?>           GetMembership(Guid teamId, Guid userId);
    Task<List<ToDo>>            GetToDosByTeamId(Guid teamId);
}
