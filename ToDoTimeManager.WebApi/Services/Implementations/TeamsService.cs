using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class TeamsService : ITeamsService
{
    private readonly ITeamsDataController _teamsDataController;
    private readonly ITeamMembersDataController _teamMembersDataController;
    private readonly IToDosDataController _toDosDataController;
    private readonly ILogger<TeamsService> _logger;

    public TeamsService(
        ITeamsDataController teamsDataController,
        ITeamMembersDataController teamMembersDataController,
        IToDosDataController toDosDataController,
        ILogger<TeamsService> logger)
    {
        _teamsDataController = teamsDataController;
        _teamMembersDataController = teamMembersDataController;
        _toDosDataController = toDosDataController;
        _logger = logger;
    }

    public async Task<List<TeamResponseDto>> GetAllTeams()
    {
        var entities = await _teamsDataController.GetAllTeams();
        return entities.Select(e => MapToDto(e.ToTeam(), null)).ToList();
    }

    public async Task<TeamResponseDto?> GetTeamById(Guid teamId)
    {
        var entity = await _teamsDataController.GetTeamById(teamId);
        if (entity == null) return null;

        var memberEntities = await _teamMembersDataController.GetMembersByTeamId(teamId);
        var members = memberEntities.Select(m => m.ToTeamMember()).ToList();
        return MapToDto(entity.ToTeam(), members);
    }

    public async Task<List<TeamResponseDto>> GetTeamsByUserId(Guid userId)
    {
        var entities = await _teamsDataController.GetTeamsByUserId(userId);
        return entities.Select(e => MapToDto(e.ToTeam(), null)).ToList();
    }

    public async Task<bool> CreateTeam(CreateTeamRequestDto request, Guid createdByUserId)
    {
        var teamEntity = new TeamEntity
        {
            Id          = request.Id,
            Name        = request.Name,
            Description = request.Description,
            CreatedAt   = DateTime.UtcNow,
            CreatedBy   = createdByUserId
        };

        var teamCreated = await _teamsDataController.CreateTeam(teamEntity);
        if (!teamCreated)
        {
            _logger.LogError("Failed to create team {TeamId}", request.Id);
            return false;
        }

        var ownerMember = new TeamMemberEntity
        {
            Id     = Guid.NewGuid(),
            TeamId = request.Id,
            UserId = createdByUserId,
            Role   = TeamMemberRole.Owner
        };

        var ownerAdded = await _teamMembersDataController.AddMember(ownerMember);
        if (!ownerAdded)
            _logger.LogError("Team {TeamId} created but failed to add owner {UserId}", request.Id, createdByUserId);

        return ownerAdded;
    }

    public async Task<bool> UpdateTeam(UpdateTeamRequestDto request)
    {
        var entity = new TeamEntity
        {
            Id          = request.Id,
            Name        = request.Name,
            Description = request.Description
        };
        return await _teamsDataController.UpdateTeam(entity);
    }

    public async Task<bool> DeleteTeam(Guid teamId)
    {
        return await _teamsDataController.DeleteTeam(teamId);
    }

    public async Task<bool> AddMember(TeamMemberUpsertRequestDto request)
    {
        var existing = await _teamMembersDataController.GetMemberByTeamIdAndUserId(request.TeamId, request.UserId);
        if (existing != null) return false;

        var entity = new TeamMemberEntity
        {
            Id     = request.Id,
            TeamId = request.TeamId,
            UserId = request.UserId,
            Role   = request.Role
        };
        return await _teamMembersDataController.AddMember(entity);
    }

    public async Task<bool> RemoveMember(Guid teamId, Guid userId)
    {
        var members = await _teamMembersDataController.GetMembersByTeamId(teamId);
        var owners = members.Where(m => m.Role == TeamMemberRole.Owner).ToList();

        var targetMember = members.FirstOrDefault(m => m.UserId == userId);
        if (targetMember == null) return false;

        if (targetMember.Role == TeamMemberRole.Owner && owners.Count == 1)
        {
            _logger.LogWarning("Cannot remove the last owner of team {TeamId}", teamId);
            return false;
        }

        return await _teamMembersDataController.RemoveMember(teamId, userId);
    }

    public async Task<TeamMember?> GetMembership(Guid teamId, Guid userId)
    {
        var entity = await _teamMembersDataController.GetMemberByTeamIdAndUserId(teamId, userId);
        return entity?.ToTeamMember();
    }

    public async Task<List<ToDo>> GetToDosByTeamId(Guid teamId)
    {
        var entities = await _toDosDataController.GetToDosByTeamId(teamId);
        return entities.Select(e => e.ToToDo()).ToList();
    }

    private static TeamResponseDto MapToDto(Team team, List<TeamMember>? members) => new()
    {
        Id          = team.Id,
        Name        = team.Name,
        Description = team.Description,
        CreatedAt   = team.CreatedAt,
        CreatedBy   = team.CreatedBy,
        MemberCount = team.MemberCount,
        Members     = members
    };
}
