using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ToDoTimeManager.WebUI.Components.PageComponents.AuthPage.Forms;

public partial class TwoFAForm : IDisposable
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;
    [Inject] private ITwoFaTimerService TwoFaTimerService { get; set; } = null!;

    private int _remainingSeconds;
    private Action<int>? _timerHandler;
    public TwoFaTimer? TwoFaTimer { get; set; }
    private string FormattedTime => TimeSpan.FromSeconds(_remainingSeconds).ToString(@"mm\:ss");

    [Parameter] public PendingTwoFaSessionState? SessionState { get; set; }
    [Parameter] public UserResponseDto? User { get; set; }
    [Parameter] public Func<AuthPageCurrentState, Task>? GoTo { get; set; }

    public string[] OtpValues { get; set; } = new string[6];

    public string Email { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public bool IsButtonDisabled => OtpValues.Any(string.IsNullOrEmpty) || IsLoading;



    protected override void OnParametersSet()
    {
        if (User is not null)
        {
            UserId = User.Id;
            Email = User.Email ?? User.UserName ?? string.Empty;
        }
        else
        {
            UserId = Guid.Empty;
            Email = string.Empty;
        }

        if (_timerHandler != null && TwoFaTimer != null)
            TwoFaTimer.OnRemainingSecondsChanged -= _timerHandler;

        TwoFaTimer = TwoFaTimerService.GetTimer(UserId);
        if (TwoFaTimer != null)
        {
            _timerHandler = seconds =>
            {
                _remainingSeconds = seconds;
                if (seconds <= 0)
                    InvokeAsync(HandleTimerExpiredAsync);
                else
                    InvokeAsync(StateHasChanged);
            };
            TwoFaTimer?.OnRemainingSecondsChanged += _timerHandler;
        }

        base.OnParametersSet();
    }

    private async Task HandleTimerExpiredAsync()
    {
        OtpValues = new string[6];
        await ProtectedLocalStorage.RemovePendingTwoFaContextAsync();
        if (GoTo != null) await GoTo(AuthPageCurrentState.Login);
    }

    private async Task OnResendCodeClicked()
    {
        await Loading(async () =>
        {
            var result = await AuthService.SendCode(new SendTwoFactorCodeRequestDto { UserId = UserId });
            if (result is null) return;
            TwoFaTimer = TwoFaTimerService.GetTimer(UserId);
            if (TwoFaTimer == null)
            {
                TwoFaTimerService.StartTimer(UserId, result.CodeLifetimeSeconds);
                TwoFaTimer = TwoFaTimerService.GetTimer(UserId);
            }
            OtpValues = new string[6];
            await JsRuntime.InvokeVoidAsync("initializeOtpInputs", "otp-inputs");
        });
    }

    private async Task OnUseDifferentEmailClicked()
    {
        OtpValues = new string[6];
        TwoFaTimer?.Dispose();
        await ProtectedLocalStorage.RemovePendingTwoFaContextAsync();
        if (GoTo != null) await GoTo(AuthPageCurrentState.Login);
    }

    private async Task OnVerifyClicked()
    {
        if (OtpValues.Any(string.IsNullOrEmpty)) return;

        await Loading(async () =>
        {
            var code = $"{OtpValues[0]}{OtpValues[1]}{OtpValues[2]}-{OtpValues[3]}{OtpValues[4]}{OtpValues[5]}";
            var tokens = await AuthService.VerifyCode(new VerifyTwoFactorRequestDto
            {
                UserId = UserId,
                Code = code,
                KeepSignedIn = SessionState?.KeepSignedIn ?? true
            });

            if (tokens is null) return;

            TwoFaTimer?.Dispose();
            await ProtectedLocalStorage.SaveLastLoginParameterAsync(Email);
            await ProtectedLocalStorage.RemovePendingTwoFaContextAsync();
            if (AuthenticationStateProvider is CustomAuthStateProvider authProvider)
                await authProvider.MarkUserAsAuthenticated(tokens);
            else
                await ProtectedLocalStorage.SaveTokenAsync(tokens);
            NavigationManager.NavigateTo(NavigationManager.BaseUri);
        });
    }

    private void HandleInput(ChangeEventArgs e, int index)
    {
        var raw = e.Value?.ToString() ?? string.Empty;
        var val = raw.Length > 1 ? raw[..1].ToUpper() : raw.ToUpper();
        OtpValues[index - 1] = val;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JsRuntime.InvokeVoidAsync("initializeOtpInputs", "otp-inputs");
    }

    private string GetIsFilledCssClass(int index) =>
        string.IsNullOrEmpty(OtpValues[index]) ? string.Empty : "filled";

    public void Dispose()
    {
        if (TwoFaTimer != null)
            TwoFaTimer?.OnRemainingSecondsChanged -= _timerHandler;
    }
}
