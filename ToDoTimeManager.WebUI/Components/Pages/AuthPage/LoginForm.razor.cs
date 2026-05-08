using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Models.Enums;

namespace ToDoTimeManager.WebUI.Components.Pages.AuthPage;

public partial class LoginForm
{
    [Parameter] public Action<AuthPageCurrentState>? GoTo { get; set; }
    [Parameter] public Action<string>? EmailChanged { get; set; }
    private string LogInParameter { get; set; } = string.Empty;
    private string Password { get; set; } = string.Empty;
    private bool KeepSignedIn { get; set; }
    public CancellationTokenSource CancellationToken { get; set; } = new();
    public string UserName { get; set; } = "";
    public bool IsPasswordValid { get; set; }
    public bool IsLogInParameterValid { get; set; }


    private async Task OnSignInClicked()
    {
        await Loading(async () =>
        {
            EmailChanged?.Invoke("newemail@gmail.com");
            GoTo?.Invoke(AuthPageCurrentState.TwoFA);
            await Task.Delay(10000, CancellationToken.Token);
        });
    }

    private void OnCreateAccountClicked()
    {
        CancellationToken.Cancel();
        GoTo?.Invoke(AuthPageCurrentState.Registration);
    }

}