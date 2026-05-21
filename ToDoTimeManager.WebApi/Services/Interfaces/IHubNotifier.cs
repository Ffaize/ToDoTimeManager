namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IHubNotifier
{
    Task NotifyUserAsync<T>(Guid userId, string method, T payload, CancellationToken ct = default);
    Task NotifyTeamAsync<T>(Guid teamId, string method, T payload, CancellationToken ct = default);
    Task NotifyProjectAsync<T>(Guid projectId, string method, T payload, CancellationToken ct = default);
    Task NotifyManagersAsync<T>(string method, T payload, CancellationToken ct = default);
}
