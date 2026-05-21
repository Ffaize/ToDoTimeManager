using Microsoft.AspNetCore.SignalR.Client;

namespace ToDoTimeManager.WebUI.Services.Services.Interfaces;

public interface ISignalRService : IAsyncDisposable
{
    HubConnectionState State { get; }
    Task StartAsync(CancellationToken ct = default);
    Task StopAsync(CancellationToken ct = default);
    IDisposable On<T>(string method, Action<T> handler);
}
