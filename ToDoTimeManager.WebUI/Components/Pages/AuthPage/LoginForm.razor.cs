using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Models;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;
using ToDoTimeManager.WebUI.Utils;

namespace ToDoTimeManager.WebUI.Components.Pages.AuthPage;

public partial class LoginForm
{
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;

    [Parameter] public Func<AuthPageCurrentState, Task>? GoTo { get; set; }
    [Parameter] public Action<PendingTwoFaSessionState, UserResponseDto>? AuthInfoChanged { get; set; }

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

            var user = new UserResponseDto
            {
                Id = result.UserId,
                Email = LogInParameter.Contains('@') ? LogInParameter : null,
                UserName = LogInParameter.Contains('@') ? null : LogInParameter
            };

            var session = new PendingTwoFaSessionState(
                result.Email ?? string.Empty,
                result.SenderEmail ?? string.Empty,
                KeepSignedIn,
                result.CodeLifetimeSeconds,
                AuthPageCurrentState.Login
            );

            await ProtectedLocalStorage.SaveUserInfoAsync(user);
            await ProtectedLocalStorage.SavePendingTwoFaSessionStateAsync(session);


            AuthInfoChanged?.Invoke(session, user);
            if (GoTo != null) await GoTo(AuthPageCurrentState.TwoFA);
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        var lastLoginParameter = await ProtectedLocalStorage.GetLastLoginParameterAsync();
        if (!string.IsNullOrEmpty(lastLoginParameter))
        {
            LogInParameter = lastLoginParameter;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task OnCreateAccountClicked()
    {
        if (GoTo != null) await GoTo(AuthPageCurrentState.Registration);
    }
}
