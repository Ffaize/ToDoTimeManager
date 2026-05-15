using Microsoft.AspNetCore.Components;

namespace ToDoTimeManager.WebUI.Components.PageComponents.AuthPage.Forms;

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
    private bool IsButtonDisabled => !IsPasswordValid || !IsLogInParameterValid || IsLoading;



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
            IsLogInParameterValid = true;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task OnCreateAccountClicked()
    {
        if (GoTo != null) await GoTo(AuthPageCurrentState.Registration);
    }
}
