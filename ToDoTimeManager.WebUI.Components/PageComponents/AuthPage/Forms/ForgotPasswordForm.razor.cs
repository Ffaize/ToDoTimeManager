using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Models.Models;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Utils.PotectedLocalStorageHelpers;

namespace ToDoTimeManager.WebUI.Components.PageComponents.AuthPage.Forms;

public partial class ForgotPasswordForm
{
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;

    [Parameter] public Func<AuthPageCurrentState, Task>? GoTo { get; set; }
    [Parameter] public Action<PendingPasswordResetState>? PasswordResetInfoChanged { get; set; }

    private string Email { get; set; } = string.Empty;
    private bool IsEmailValid { get; set; }
    private bool IsButtonDisabled => !IsEmailValid || IsLoading;

    private async Task OnSendResetCodeClicked()
    {
        if (string.IsNullOrWhiteSpace(Email)) return;

        await Loading(async () =>
        {
            var result = await AuthService.ForgotPassword(Email);
            if (result is null) return;

            var state = new PendingPasswordResetState
            {
                UserId = result.UserId,
                MaskedEmail = result.MaskedEmail ?? Email,
                CodeLifetimeSeconds = result.CodeLifetimeSeconds
            };

            await ProtectedLocalStorage.SavePendingPasswordResetStateAsync(state);
            PasswordResetInfoChanged?.Invoke(state);
            if (GoTo != null) await GoTo(AuthPageCurrentState.ResetPassword);
        });
    }

    private async Task OnBackToLoginClicked()
    {
        if (GoTo != null) await GoTo(AuthPageCurrentState.Login);
    }
}
