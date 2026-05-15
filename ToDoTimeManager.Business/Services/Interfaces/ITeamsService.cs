using ToDoTimeManager.Shared.DTOs.Team;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Interfaces;

public interface ITeamsService
{
    Task<List<TeamResponseDto>> GetAllTeams();
    Task<TeamResponseDto?> GetTeamById(Guid teamId, Guid currentUserId, UserRole currentUserRole);
    Task<List<TeamResponseDto>> GetTeamsByUserId(Guid userId);
    Task<bool> CreateTeam(CreateTeamRequestDto request, Guid createdByUserId);
    Task<bool> UpdateTeam(UpdateTeamRequestDto request, Guid currentUserId, UserRole currentUserRole);
    Task<bool> DeleteTeam(Guid teamId);
    Task<bool> AddMember(TeamMemberUpsertRequestDto request, Guid currentUserId, UserRole currentUserRole);
    Task<bool> RemoveMember(Guid teamId, Guid userId, Guid currentUserId, UserRole currentUserRole);
    Task<List<ToDo>> GetToDosByTeamId(Guid teamId, Guid currentUserId, UserRole currentUserRole);
}
