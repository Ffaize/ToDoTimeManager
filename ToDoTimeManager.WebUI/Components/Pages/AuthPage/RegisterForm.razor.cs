using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Models.Enums;

namespace ToDoTimeManager.WebUI.Components.Pages.AuthPage;

public partial class RegisterForm
{
    [Parameter] public Action<AuthPageCurrentState>? GoTo { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public bool IsUseOfTermsAgreed { get; set; }
    public bool IsConfirmPasswordValid { get; set; }
    public bool IsPasswordValid { get; set; }
    public bool IsEmailValid { get; set; }
    public bool IsUsernameValid { get; set; }

    private void OnSignInClicked()
    {
        GoTo?.Invoke(AuthPageCurrentState.Login);
    }
}