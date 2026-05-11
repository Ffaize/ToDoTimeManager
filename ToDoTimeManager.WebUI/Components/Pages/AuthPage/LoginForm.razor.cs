using Microsoft.AspNetCore.Components;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.HttpServices;

namespace ToDoTimeManager.WebUI.Components.Pages.AuthPage;

public partial class LoginForm
{
    [Inject] private AuthService AuthService { get; set; } = null!;

    [Parameter] public Action<AuthPageCurrentState>? GoTo { get; set; }
    [Parameter] public Action<(string Email, Guid UserId)>? UserChanged { get; set; }

    private string LogInParameter { get; set; } = string.Empty;
    private string Password { get; set; } = string.Empty;
    private bool KeepSignedIn { get; set; }
    public CancellationTokenSource CancellationToken { get; set; } = new();
    public bool IsPasswordValid { get; set; }
    public bool IsLogInParameterValid { get; set; }

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

            UserChanged?.Invoke((result.MaskedEmail ?? string.Empty, result.UserId));
            GoTo?.Invoke(AuthPageCurrentState.TwoFA);
        });
    }

    private void OnCreateAccountClicked()
    {
        CancellationToken.Cancel();
        GoTo?.Invoke(AuthPageCurrentState.Registration);
    }
}
