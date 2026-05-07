using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Models.Enums;

namespace ToDoTimeManager.WebUI.Components.Pages.AuthPage;

public partial class LoginForm
{
    [Parameter] public Action<AuthPageCurrentState>? GoTo { get; set; }
    private string Email { get; set; } = string.Empty;
    private string Password { get; set; } = string.Empty;
    private bool KeepSignedIn { get; set; }
    public CancellationTokenSource CancellationToken { get; set; } = new();


    private async Task OnSignInClicked()
    {
        await Loading(async () =>
        {
            await Task.Delay(10000, CancellationToken.Token);
        });
    }

    private void OnCreateAccountClicked()
    {
        CancellationToken.Cancel();
        GoTo?.Invoke(AuthPageCurrentState.Registration);
    }

}