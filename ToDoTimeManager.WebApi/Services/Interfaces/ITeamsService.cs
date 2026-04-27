using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface ITeamsService
{
    Task<List<TeamResponseDto>> GetAllTeams();
    Task<TeamResponseDto?> GetTeamById(Guid teamId, Guid currentUserId, bool isAdmin);
    Task<List<TeamResponseDto>> GetTeamsByUserId(Guid userId);
    Task<bool> CreateTeam(CreateTeamRequestDto request, Guid createdByUserId);
    Task<bool> UpdateTeam(UpdateTeamRequestDto request, Guid currentUserId, bool isAdmin);
    Task<bool> DeleteTeam(Guid teamId);
    Task<bool> AddMember(TeamMemberUpsertRequestDto request, Guid currentUserId, bool isAdmin);
    Task<bool> RemoveMember(Guid teamId, Guid userId, Guid currentUserId, bool isAdmin);
    Task<List<ToDo>> GetToDosByTeamId(Guid teamId, Guid currentUserId, bool isAdmin);
}