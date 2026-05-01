using System.Security.Claims;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class AccessControlService : IAccessControlService
{
    private readonly IAccessControlDataController _accessControlDataController;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AccessControlService> _logger;

    public AccessControlService(
        IAccessControlDataController accessControlDataController,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AccessControlService> logger)
    {
        _accessControlDataController = accessControlDataController;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<bool> IsAccessibleToUser(Guid userId, Guid objectId, string methodName)
    {
        try
        {
            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
            if (Enum.TryParse<UserRole>(roleClaim, out var role) && role >= UserRole.Manager)
                return true;

            return methodName switch
            {
                // Project-level checks (objectId = projectId)
                "GetProjectById" or "GetToDosByProjectId" or "UpdateProject"
                    or "AddTeam" or "RemoveTeam" or "CreateToDo" or "UpdateToDo"
                    => objectId == Guid.Empty || await _accessControlDataController.CanAccessProject(userId, objectId),

                // Team-level checks (objectId = teamId)
                "GetTeamById" or "GetToDosByTeamId"
                    => await _accessControlDataController.CanAccessTeam(userId, objectId),

                // ToDo-level checks (objectId = toDoId)
                "GetToDoById" or "DeleteToDo" or "GetTimeLogsByToDoId"
                    or "GetTimeLogsByUserIdAndToDoId" or "CreateTimeLog"
                    => await _accessControlDataController.CanAccessToDo(userId, objectId),

                // TimeLog-level checks (objectId = timeLogId)
                "GetTimeLogById" or "UpdateTimeLog" or "DeleteTimeLog"
                    => await _accessControlDataController.CanAccessTimeLog(userId, objectId),

                // Self-access checks (objectId = targetUserId)
                "GetToDosByUserId" or "GetTimeLogsByUserId" or "GetUserById"
                    or "GetUserByUsername" or "GetUserByEmail"
                    or "GetUserByLoginParameter" or "UpdateUser"
                    => userId == objectId,

                _ => false
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> CanAccessProject(Guid userId, Guid projectId)
    {
        try
        {
            return await _accessControlDataController.CanAccessProject(userId, projectId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> CanAccessToDo(Guid userId, Guid toDoId)
    {
        try
        {
            return await _accessControlDataController.CanAccessToDo(userId, toDoId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> CanAccessTimeLog(Guid userId, Guid timeLogId)
    {
        try
        {
            return await _accessControlDataController.CanAccessTimeLog(userId, timeLogId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> CanAccessTeam(Guid userId, Guid teamId)
    {
        try
        {
            return await _accessControlDataController.CanAccessTeam(userId, teamId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}
