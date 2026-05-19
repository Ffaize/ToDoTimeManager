using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Models.Models;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Services.Interfaces;
using ToDoTimeManager.WebUI.Utils.PotectedLocalStorageHelpers;

namespace ToDoTimeManager.WebUI.Components.PageComponents.AuthPage.Forms;

public partial class ResetPasswordForm : IDisposable
{
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;
    [Inject] private ITwoFaTimerService TwoFaTimerService { get; set; } = null!;
    [Inject] private IToastsService ToastsService { get; set; } = null!;

    [Parameter] public Func<AuthPageCurrentState, Task>? GoTo { get; set; }
    [Parameter] public PendingPasswordResetState? SessionState { get; set; }

    private int _remainingSeconds;
    private Action<int>? _timerHandler;
    private TwoFaTimer? _timer;

    private string FormattedTime => TimeSpan.FromSeconds(_remainingSeconds).ToString(@"mm\:ss");

    public string[] OtpValues { get; set; } = new string[6];
    private string NewPassword { get; set; } = string.Empty;
    private string ConfirmPassword { get; set; } = string.Empty;
    private bool IsNewPasswordValid { get; set; }
    private bool IsConfirmPasswordValid { get; set; }

    private bool IsButtonDisabled =>
        OtpValues.Any(string.IsNullOrEmpty) || !IsNewPasswordValid || !IsConfirmPasswordValid || IsLoading;

    protected override void OnParametersSet()
    {
        if (_timerHandler != null && _timer != null)
            _timer.OnRemainingSecondsChanged -= _timerHandler;

        var timerId = SessionState?.UserId ?? Guid.Empty;
        _timer = TwoFaTimerService.GetTimer(timerId);
        if (_timer != null)
        {
            var culture = CultureInfo.CurrentCulture;
            var uiCulture = CultureInfo.CurrentUICulture;
            _timerHandler = seconds =>
            {
                if (seconds <= 0)
                    InvokeAsync(async () =>
                    {
                        CultureInfo.CurrentCulture = culture;
                        CultureInfo.CurrentUICulture = uiCulture;
                        _remainingSeconds = seconds;
                        await HandleTimerExpiredAsync();
                    });
                else
                    InvokeAsync(() =>
                    {
                        CultureInfo.CurrentCulture = culture;
                        CultureInfo.CurrentUICulture = uiCulture;
                        _remainingSeconds = seconds;
                        StateHasChanged();
                    });
            };
            _timer.OnRemainingSecondsChanged += _timerHandler;
        }

        base.OnParametersSet();
    }

    private async Task HandleTimerExpiredAsync()
    {
        OtpValues = new string[6];
        await ProtectedLocalStorage.RemovePendingPasswordResetContextAsync();
        await ToastsService.ShowToast(Localizer["ResetPassword_CodeExpired"], ToastType.Error);
        if (GoTo != null) await GoTo(AuthPageCurrentState.ForgotPassword);
    }

    private async Task OnResetClicked()
    {
        if (OtpValues.Any(string.IsNullOrEmpty) || SessionState is null) return;

        await Loading(async () =>
        {
            var code = $"{OtpValues[0]}{OtpValues[1]}{OtpValues[2]}-{OtpValues[3]}{OtpValues[4]}{OtpValues[5]}";
            var success = await AuthService.ResetPassword(SessionState.UserId, code, NewPassword);
            if (!success) return;

            _timer?.Dispose();
            await ProtectedLocalStorage.RemovePendingPasswordResetContextAsync();
            TwoFaTimerService.RemoveTimer(SessionState.UserId);
            await ToastsService.ShowToast(Localizer["ResetPassword_Success"], ToastType.Success);
            if (GoTo != null) await GoTo(AuthPageCurrentState.Login);
        });
    }

    private async Task OnUseDifferentEmailClicked()
    {
        OtpValues = new string[6];
        _timer?.Dispose();
        if (SessionState is not null)
            TwoFaTimerService.RemoveTimer(SessionState.UserId);
        await ProtectedLocalStorage.RemovePendingPasswordResetContextAsync();
        if (GoTo != null) await GoTo(AuthPageCurrentState.ForgotPassword);
    }


    public async Task InvokePrimaryAsync()
    {
        if (!IsButtonDisabled)
            await OnResetClicked();
    }

    public void Dispose()
    {
        if (_timer != null && _timerHandler != null)
            _timer.OnRemainingSecondsChanged -= _timerHandler;
    }
}
