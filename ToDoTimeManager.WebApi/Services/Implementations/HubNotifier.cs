using Microsoft.AspNetCore.SignalR;
using ToDoTimeManager.WebApi.Hubs;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class HubNotifier(IHubContext<CommunicationHub> hubContext) : IHubNotifier
{
    public Task NotifyUserAsync<T>(Guid userId, string method, T payload, CancellationToken ct = default)
        => hubContext.Clients.Group(CommunicationHub.UserGroup(userId)).SendAsync(method, payload, cancellationToken: ct);

    public Task NotifyTeamAsync<T>(Guid teamId, string method, T payload, CancellationToken ct = default)
        => hubContext.Clients.Group(CommunicationHub.TeamGroup(teamId)).SendAsync(method, payload, cancellationToken: ct);

    public Task NotifyProjectAsync<T>(Guid projectId, string method, T payload, CancellationToken ct = default)
        => hubContext.Clients.Group(CommunicationHub.ProjectGroup(projectId)).SendAsync(method, payload, cancellationToken: ct);

    public Task NotifyManagersAsync<T>(string method, T payload, CancellationToken ct = default)
        => hubContext.Clients.Group(CommunicationHub.ManagersGroup).SendAsync(method, payload, cancellationToken: ct);
}
