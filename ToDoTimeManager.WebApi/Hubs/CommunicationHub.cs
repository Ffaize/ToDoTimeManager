using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.WebApi.Hubs;

[Authorize]
public class CommunicationHub(ITeamsService teamsService, IProjectsService projectsService) : Hub
{
    public const string ManagersGroup = "role-managers";
    public static string UserGroup(Guid userId)       => $"user-{userId}";
    public static string TeamGroup(Guid teamId)       => $"team-{teamId}";
    public static string ProjectGroup(Guid projectId) => $"project-{projectId}";

    public override async Task OnConnectedAsync()
    {
        var userIdClaim = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var roleClaim   = Context.User?.FindFirstValue(ClaimTypes.Role);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            await base.OnConnectedAsync();
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));

        if (Enum.TryParse<UserRole>(roleClaim, out var role) && role >= UserRole.Manager)
            await Groups.AddToGroupAsync(Context.ConnectionId, ManagersGroup);

        var teamIds = await teamsService.GetTeamIdsByUserId(userId);
        foreach (var id in teamIds)
            await Groups.AddToGroupAsync(Context.ConnectionId, TeamGroup(id));

        var projectIds = await projectsService.GetProjectIdsByUserId(userId);
        foreach (var id in projectIds)
            await Groups.AddToGroupAsync(Context.ConnectionId, ProjectGroup(id));

        await base.OnConnectedAsync();
    }
}
