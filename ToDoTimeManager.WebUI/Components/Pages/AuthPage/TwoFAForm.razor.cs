using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;
using ToDoTimeManager.WebUI.Services.Interfaces;
using ToDoTimeManager.WebUI.Utils;

namespace ToDoTimeManager.WebUI.Components.Pages.AuthPage;

public partial class TwoFAForm : IDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = null!;
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;
    [Inject] private ITwoFaTimerService TwoFaTimerService { get; set; } = null!;

    private CancellationTokenSource _cts = new();
    private int _remainingSeconds;
    private string FormattedTime => TimeSpan.FromSeconds(_remainingSeconds).ToString(@"mm\:ss");

    [Parameter] public Func<AuthPageCurrentState, Task>? GoTo { get; set; }
    [Parameter] public string Email { get; set; } = string.Empty;
    [Parameter] public Guid UserId { get; set; }
    [Parameter] public AuthPageCurrentState SourceState { get; set; } = AuthPageCurrentState.Login;
    [Parameter] public bool KeepSignedIn { get; set; } = true;

    protected override Task OnParametersSetAsync()
    {
        if (UserId == Guid.Empty) return Task.CompletedTask;
        _remainingSeconds = TwoFaTimerService.GetRemainingSeconds(UserId);
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        _ = StartCountdown(_cts.Token);
        return Task.CompletedTask;
    }

    private async Task StartCountdown(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        while (await timer.WaitForNextTickAsync(ct))
        {
            _remainingSeconds = TwoFaTimerService.GetRemainingSeconds(UserId);
            await InvokeAsync(StateHasChanged);
            if (_remainingSeconds <= 0) break;
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }

    public string Value1 { get; set; } = string.Empty;
    public string Value2 { get; set; } = string.Empty;
    public string Value3 { get; set; } = string.Empty;
    public string Value4 { get; set; } = string.Empty;
    public string Value5 { get; set; } = string.Empty;
    public string Value6 { get; set; } = string.Empty;

    private async Task OnResendCodeClicked()
    {
        await Loading(async () =>
        {
            var result = await AuthService.SendCode(new SendTwoFactorCodeRequestDto { UserId = UserId });
            if (result is null) return;
            TwoFaTimerService.StartTimer(UserId);
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            _ = StartCountdown(_cts.Token);
            Value1 = Value2 = Value3 = Value4 = Value5 = Value6 = string.Empty;
            await JS.InvokeVoidAsync("initializeOtpInputs", "otp-inputs");
        });
    }

    private async Task OnUseDifferentEmailClicked()
    {
        Value1 = Value2 = Value3 = Value4 = Value5 = Value6 = string.Empty;
        if (GoTo != null) await GoTo(SourceState);
    }

    private async Task OnVerifyClicked()
    {
        if (string.IsNullOrEmpty(Value1) || string.IsNullOrEmpty(Value2) || string.IsNullOrEmpty(Value3) ||
            string.IsNullOrEmpty(Value4) || string.IsNullOrEmpty(Value5) || string.IsNullOrEmpty(Value6)) return;

        await Loading(async () =>
        {
            var code = $"{Value1}{Value2}{Value3}-{Value4}{Value5}{Value6}";
            var tokens = await AuthService.VerifyCode(new VerifyTwoFactorRequestDto
            {
                UserId = UserId,
                Code = code,
                KeepSignedIn = KeepSignedIn
            });

            if (tokens is null) return;

            TwoFaTimerService.ClearTimer(UserId);
            await ProtectedLocalStorage.SaveLastLoginParameterAsync(Email);
            await ProtectedLocalStorage.RemoveAuthPageStateAsync();
            if (AuthenticationStateProvider is CustomAuthStateProvider authProvider)
                await authProvider.MarkUserAsAuthenticated(tokens);
            NavigationManager.NavigateTo("/dashboard");
        });
    }

    private void HandleInput(ChangeEventArgs e, int index)
    {
        var raw = e.Value?.ToString() ?? string.Empty;
        var val = raw.Length > 1 ? raw[..1].ToUpper() : raw.ToUpper();
        switch (index)
        {
            case 1: Value1 = val; break;
            case 2: Value2 = val; break;
            case 3: Value3 = val; break;
            case 4: Value4 = val; break;
            case 5: Value5 = val; break;
            case 6: Value6 = val; break;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JS.InvokeVoidAsync("initializeOtpInputs", "otp-inputs");
    }

    private string GetIsFilledCssClass(string value)
    {
        return string.IsNullOrEmpty(value) ? string.Empty : "filled";
    }
}