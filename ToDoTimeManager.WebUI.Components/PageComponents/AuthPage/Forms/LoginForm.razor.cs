using Microsoft.AspNetCore.Components;

namespace ToDoTimeManager.WebUI.Components.PageComponents.AuthPage.Forms;

public partial class LoginForm
{
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;

    [Parameter] public Func<AuthPageCurrentState, Task>? GoTo { get; set; }
    [Parameter] public Action<PendingTwoFaSessionState, UserResponseDto>? AuthInfoChanged { get; set; }
    [Parameter] public Action<string>? OnGoToForgotPassword { get; set; }

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
                Password = Password,
                KeepSignedIn = KeepSignedIn
            });

            if (result is null) return;

            if (result.RequiresTwoFactor && result.TwoFaPending is not null)
            {
                var user = new UserResponseDto
                {
                    Id = result.TwoFaPending.UserId,
                    Email = LogInParameter.Contains('@') ? LogInParameter : null,
                    UserName = LogInParameter.Contains('@') ? null : LogInParameter
                };

                var session = new PendingTwoFaSessionState(
                    result.TwoFaPending.Email ?? string.Empty,
                    result.TwoFaPending.SenderEmail ?? string.Empty,
                    KeepSignedIn,
                    result.TwoFaPending.CodeLifetimeSeconds,
                    AuthPageCurrentState.Login,
                    result.TwoFaPending.TwoFactorMethod
                );

                await ProtectedLocalStorage.SaveUserInfoAsync(user);
                await ProtectedLocalStorage.SavePendingTwoFaSessionStateAsync(session);

                AuthInfoChanged?.Invoke(session, user);
                if (GoTo != null) await GoTo(AuthPageCurrentState.TwoFA);
            }
            else if (result.Token is not null)
            {
                await ProtectedLocalStorage.SaveLastLoginParameterAsync(LogInParameter);
                if (AuthenticationStateProvider is CustomAuthStateProvider authProvider)
                    await authProvider.MarkUserAsAuthenticated(result.Token);
                NavigationManager.NavigateTo(NavigationManager.BaseUri);
            }
        });
    }

    protected override async Task OnInitializedAsync()
    {
        var lastLoginParameter = await ProtectedLocalStorage.GetLastLoginParameterAsync();
        if (!string.IsNullOrEmpty(lastLoginParameter))
        {
            LogInParameter = lastLoginParameter;
            IsLogInParameterValid = true;
            await InvokeAsync(StateHasChanged);
        }
    }

    private void OnGoogleLoginClicked()
    {
        NavigationManager.NavigateTo(AuthService.GetGoogleLoginUrl(), forceLoad: true);
    }

    private void OnGitHubLoginClicked()
    {
        NavigationManager.NavigateTo(AuthService.GetGitHubLoginUrl(), forceLoad: true);
    }

    private async Task OnCreateAccountClicked()
    {
        if (GoTo != null) await GoTo(AuthPageCurrentState.Registration);
    }

    private async Task OnForgotPasswordClicked()
    {
        OnGoToForgotPassword?.Invoke(LogInParameter);
        if (GoTo != null) await GoTo(AuthPageCurrentState.ForgotPassword);
    }

    public async Task InvokePrimaryAsync()
    {
        if (!IsButtonDisabled)
            await OnSignInClicked();
    }
}
