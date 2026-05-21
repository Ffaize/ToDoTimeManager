using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ToDoTimeManager.WebUI.Services.Services.Interfaces;

namespace ToDoTimeManager.WebUI.Components.Shared;

public partial class SignalRConnector : IAsyncDisposable
{
    [Inject] private ISignalRService SignalRService { get; set; } = default!;
    [Inject] private AuthenticationStateProvider Auth { get; set; } = default!;

    private bool _started;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        var state = await Auth.GetAuthenticationStateAsync();
        if (state.User.Identity?.IsAuthenticated == true)
            await StartAsync();

        Auth.AuthenticationStateChanged += OnAuthChanged;
    }

    private async void OnAuthChanged(Task<AuthenticationState> task)
    {
        try
        {
            var state = await task;
            if (state.User.Identity?.IsAuthenticated == true)
            {
                if (!_started) await StartAsync();
            }
            else
            {
                await StopAsync();
            }
        }
        catch { /* не крашимо circuit */ }
    }

    private async Task StartAsync()
    {
        await SignalRService.StartAsync();
        _started = true;
    }

    private async Task StopAsync()
    {
        await SignalRService.StopAsync();
        _started = false;
    }

    public async ValueTask DisposeAsync()
    {
        Auth.AuthenticationStateChanged -= OnAuthChanged;
        await SignalRService.StopAsync();
    }
}
