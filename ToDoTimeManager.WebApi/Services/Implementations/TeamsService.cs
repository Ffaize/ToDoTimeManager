using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Exceptions;
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

    public async Task<TeamResponseDto?> GetTeamById(Guid teamId, Guid currentUserId, bool isAdmin)
    {
        if (teamId == Guid.Empty)
            throw new ValidationException("Invalid team ID");

        try
        {
            var entity = await _teamsDataController.GetTeamById(teamId);
            if (entity == null)
                throw new NotFoundException("Team was not found");

            if (!isAdmin)
            {
                var membership = await _teamMembersDataController.GetMemberByTeamIdAndUserId(teamId, currentUserId);
                if (membership == null)
                    throw new ForbiddenException();
            }

            var memberEntities = await _teamMembersDataController.GetMembersByTeamId(teamId);
            var members = memberEntities.Select(m => m.ToTeamMember()).ToList();
            return MapToDto(entity.ToTeam(), members);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<List<TeamResponseDto>> GetTeamsByUserId(Guid userId)
    {
        var entities = await _teamsDataController.GetTeamsByUserId(userId);
        return entities.Select(e => MapToDto(e.ToTeam(), null)).ToList();
    }

    public async Task<bool> CreateTeam(CreateTeamRequestDto request, Guid createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Team name is required");

        var teamEntity = new TeamEntity
        {
            Id          = request.Id,
            Name        = request.Name,
            Description = request.Description,
            CreatedAt   = DateTime.UtcNow,
            CreatedBy   = createdByUserId
        };

        try
        {
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
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateTeam(UpdateTeamRequestDto request, Guid currentUserId, bool isAdmin)
    {
        if (request.Id == Guid.Empty)
            throw new ValidationException("Invalid team ID");

        try
        {
            if (!isAdmin)
            {
                var membership = await _teamMembersDataController.GetMemberByTeamIdAndUserId(request.Id, currentUserId);
                if (membership == null || membership.Role != TeamMemberRole.Owner)
                    throw new ForbiddenException();
            }

            var entity = new TeamEntity
            {
                Id          = request.Id,
                Name        = request.Name,
                Description = request.Description
            };
            return await _teamsDataController.UpdateTeam(entity);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteTeam(Guid teamId)
    {
        if (teamId == Guid.Empty)
            throw new ValidationException("Invalid team ID");

        try
        {
            return await _teamsDataController.DeleteTeam(teamId);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> AddMember(TeamMemberUpsertRequestDto request, Guid currentUserId, bool isAdmin)
    {
        if (request.TeamId == Guid.Empty)
            throw new ValidationException("Invalid team ID");

        try
        {
            if (!isAdmin)
            {
                var callerMembership = await _teamMembersDataController.GetMemberByTeamIdAndUserId(request.TeamId, currentUserId);
                if (callerMembership == null || callerMembership.Role != TeamMemberRole.Owner)
                    throw new ForbiddenException();
            }

            var existing = await _teamMembersDataController.GetMemberByTeamIdAndUserId(request.TeamId, request.UserId);
            if (existing != null)
                throw new ConflictException("User is already a member of this team");

            var entity = new TeamMemberEntity
            {
                Id     = request.Id,
                TeamId = request.TeamId,
                UserId = request.UserId,
                Role   = request.Role
            };
            return await _teamMembersDataController.AddMember(entity);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> RemoveMember(Guid teamId, Guid userId, Guid currentUserId, bool isAdmin)
    {
        if (teamId == Guid.Empty || userId == Guid.Empty)
            throw new ValidationException("Invalid team or user ID");

        try
        {
            if (!isAdmin)
            {
                var callerMembership = await _teamMembersDataController.GetMemberByTeamIdAndUserId(teamId, currentUserId);
                if (callerMembership == null || callerMembership.Role != TeamMemberRole.Owner)
                    throw new ForbiddenException();
            }

            var members = await _teamMembersDataController.GetMembersByTeamId(teamId);
            var targetMember = members.FirstOrDefault(m => m.UserId == userId);
            if (targetMember == null)
                throw new NotFoundException("Member was not found in this team");

            var owners = members.Where(m => m.Role == TeamMemberRole.Owner).ToList();
            if (targetMember.Role == TeamMemberRole.Owner && owners.Count == 1)
                throw new ConflictException("Cannot remove the last owner of a team");

            return await _teamMembersDataController.RemoveMember(teamId, userId);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<List<ToDo>> GetToDosByTeamId(Guid teamId, Guid currentUserId, bool isAdmin)
    {
        if (teamId == Guid.Empty)
            throw new ValidationException("Invalid team ID");

        try
        {
            if (!isAdmin)
            {
                var membership = await _teamMembersDataController.GetMemberByTeamIdAndUserId(teamId, currentUserId);
                if (membership == null)
                    throw new ForbiddenException();
            }

            var entities = await _toDosDataController.GetToDosByTeamId(teamId);
            return entities.Select(e => e.ToToDo()).ToList();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
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
