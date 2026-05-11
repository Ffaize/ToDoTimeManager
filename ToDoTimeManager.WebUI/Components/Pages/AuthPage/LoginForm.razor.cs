using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Utils;

namespace ToDoTimeManager.WebUI.Components.Pages.AuthPage;

public partial class LoginForm
{
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;

    [Parameter] public Func<AuthPageCurrentState, Task>? GoTo { get; set; }
    [Parameter] public Action<(string Email, Guid UserId, bool KeepSignedIn)>? UserChanged { get; set; }

    private string LogInParameter { get; set; } = string.Empty;
    private string Password { get; set; } = string.Empty;
    private bool KeepSignedIn { get; set; } = true;
    private bool IsPasswordValid { get; set; }
    private bool IsLogInParameterValid { get; set; }

    private async Task OnSignInClicked()
    {
        if (string.IsNullOrWhiteSpace(LogInParameter) || string.IsNullOrWhiteSpace(Password)) return;

        await Loading(async () =>
        {
            var result = await AuthService.Login(new LoginUser
            {
                LoginParameter = LogInParameter,
                Password = Password
            });

            if (result is null) return;

            UserChanged?.Invoke((result.MaskedEmail ?? string.Empty, result.UserId, KeepSignedIn));
            if (GoTo != null) await GoTo(AuthPageCurrentState.TwoFA);
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        var lastLoginParameter = await ProtectedLocalStorage.GetLastLoginParameterAsync();
        if (!string.IsNullOrEmpty(lastLoginParameter))
            LogInParameter = lastLoginParameter;
    }

    private async Task OnCreateAccountClicked()
    {
        if (GoTo != null) await GoTo(AuthPageCurrentState.Registration);
    }
}
