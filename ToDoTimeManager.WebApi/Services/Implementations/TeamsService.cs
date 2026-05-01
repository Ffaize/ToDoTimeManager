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
    private readonly IToDosService _toDosService;
    private readonly IAccessControlService _accessControlService;
    private readonly ILogger<TeamsService> _logger;

    public TeamsService(
        ITeamsDataController teamsDataController,
        ITeamMembersDataController teamMembersDataController,
        IToDosService toDosService,
        IAccessControlService accessControlService,
        ILogger<TeamsService> logger)
    {
        _teamsDataController = teamsDataController;
        _teamMembersDataController = teamMembersDataController;
        _toDosService = toDosService;
        _accessControlService = accessControlService;
        _logger = logger;
    }

    public async Task<List<TeamResponseDto>> GetAllTeams()
    {
        List<TeamEntity> entities = await _teamsDataController.GetAllTeams();
        return entities.Select(e => MapToDto(e.ToTeam(), null)).ToList();
    }

    public async Task<TeamResponseDto?> GetTeamById(Guid teamId, Guid currentUserId, UserRole currentUserRole)
    {
        if (teamId == Guid.Empty)
            throw new ValidationException("Invalid team ID");

        try
        {
            var entity = await _teamsDataController.GetTeamById(teamId);
            if (entity == null)
                throw new NotFoundException("Team was not found");

            if (!await _accessControlService.IsAccessibleToUser(currentUserId, teamId, nameof(GetTeamById)))
                throw new ForbiddenException();

            List<TeamMemberEntity> memberEntities = await _teamMembersDataController.GetMembersByTeamId(teamId);
            List<TeamMember> members = memberEntities.Select(m => m.ToTeamMember()).ToList();
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
        List<TeamEntity> entities = await _teamsDataController.GetTeamsByUserId(userId);
        return entities.Select(e => MapToDto(e.ToTeam(), null)).ToList();
    }

    public async Task<bool> CreateTeam(CreateTeamRequestDto request, Guid createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Team name is required");

        var teamEntity = new TeamEntity
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdByUserId
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
                Id = Guid.NewGuid(),
                TeamId = request.Id,
                UserId = createdByUserId,
                Role = TeamMemberRole.Owner
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

    public async Task<bool> UpdateTeam(UpdateTeamRequestDto request, Guid currentUserId, UserRole currentUserRole)
    {
        if (request.Id == Guid.Empty)
            throw new ValidationException("Invalid team ID");

        try
        {
            var entity = new TeamEntity
            {
                Id = request.Id,
                Name = request.Name,
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

    public async Task<bool> AddMember(TeamMemberUpsertRequestDto request, Guid currentUserId, UserRole currentUserRole)
    {
        if (request.TeamId == Guid.Empty)
            throw new ValidationException("Invalid team ID");

        try
        {
            var existing = await _teamMembersDataController.GetMemberByTeamIdAndUserId(request.TeamId, request.UserId);
            if (existing != null)
                throw new ConflictException("User is already a member of this team");

            var entity = new TeamMemberEntity
            {
                Id = request.Id,
                TeamId = request.TeamId,
                UserId = request.UserId,
                Role = request.Role
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

    public async Task<bool> RemoveMember(Guid teamId, Guid userId, Guid currentUserId, UserRole currentUserRole)
    {
        if (teamId == Guid.Empty || userId == Guid.Empty)
            throw new ValidationException("Invalid team or user ID");

        try
        {
            List<TeamMemberEntity> members = await _teamMembersDataController.GetMembersByTeamId(teamId);
            var targetMember = members.FirstOrDefault(m => m.UserId == userId);
            if (targetMember == null)
                throw new NotFoundException("Member was not found in this team");

            List<TeamMemberEntity> owners = members.Where(m => m.Role == TeamMemberRole.Owner).ToList();
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

    public async Task<List<ToDo>> GetToDosByTeamId(Guid teamId, Guid currentUserId, UserRole currentUserRole)
    {
        if (teamId == Guid.Empty)
            throw new ValidationException("Invalid team ID");

        try
        {
            if (!await _accessControlService.IsAccessibleToUser(currentUserId, teamId, nameof(GetToDosByTeamId)))
                throw new ForbiddenException();

            return await _toDosService.GetToDosByTeamId(teamId);
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

    private static TeamResponseDto MapToDto(Team team, List<TeamMember>? members)
    {
        return new TeamResponseDto
        {
            Id = team.Id,
            Name = team.Name,
            Description = team.Description,
            CreatedAt = team.CreatedAt,
            CreatedBy = team.CreatedBy,
            MemberCount = team.MemberCount,
            Members = members
        };
    }
}
