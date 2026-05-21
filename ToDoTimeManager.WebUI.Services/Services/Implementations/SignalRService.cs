using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ToDoTimeManager.WebUI.Services.Helpers.CircuitServicesAccesor;
using ToDoTimeManager.WebUI.Services.Services.Interfaces;
using ToDoTimeManager.WebUI.Utils.PotectedLocalStorageHelpers;

namespace ToDoTimeManager.WebUI.Services.Services.Implementations;

public class SignalRService : ISignalRService
{
    private readonly CircuitServicesAccesor _accessor;
    private readonly ILogger<SignalRService> _logger;
    private readonly HubConnection _connection;

    public HubConnectionState State => _connection.State;

    public SignalRService(
        CircuitServicesAccesor accessor,
        IConfiguration configuration,
        ILogger<SignalRService> logger)
    {
        _accessor = accessor;
        _logger   = logger;

        var apiBase = configuration["BaseApiUrlAddress"]?.TrimEnd('/') ?? "https://webapi";
        var hubUrl  = $"{apiBase}/hubs/communication";

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options => { options.AccessTokenProvider = GetAccessTokenAsync; })
            .WithAutomaticReconnect([
                TimeSpan.Zero,
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30)
            ])
            .Build();

        _connection.Closed      += ex => { if (ex != null) _logger.LogWarning(ex, "SignalR closed with error."); return Task.CompletedTask; };
        _connection.Reconnecting += ex => { _logger.LogInformation("SignalR reconnecting: {Reason}", ex?.Message); return Task.CompletedTask; };
        _connection.Reconnected  += id => { _logger.LogInformation("SignalR reconnected. ConnectionId={Id}", id); return Task.CompletedTask; };
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        if (_connection.State is HubConnectionState.Connected or HubConnectionState.Connecting)
            return;

        try
        {
            await _connection.StartAsync(ct);

            if (_connection.State != HubConnectionState.Connected)
                throw new InvalidOperationException($"SignalR connection did not reach the connected state. Current state: {_connection.State}.");

            _logger.LogInformation("SignalR connected.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start SignalR connection.");
            throw;
        }
    }

    public async Task StopAsync(CancellationToken ct = default)
    {
        try { await _connection.StopAsync(ct); }
        catch (Exception ex) { _logger.LogWarning(ex, "Error stopping SignalR connection."); }
    }

    public IDisposable On<T>(string method, Action<T> handler)
        => _connection.On(method, handler);

    private async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            var storage = _accessor.Service?.GetService<ProtectedLocalStorage>();
            if (storage == null) return null;
            var tokens = await storage.GetTokenAsync();
            return tokens?.AccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve access token for SignalR.");
            return null;
        }
    }

    public async ValueTask DisposeAsync()
        => await _connection.DisposeAsync();
}
