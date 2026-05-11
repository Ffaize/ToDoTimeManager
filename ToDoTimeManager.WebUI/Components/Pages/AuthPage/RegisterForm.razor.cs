using Microsoft.AspNetCore.Components;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.HttpServices;

namespace ToDoTimeManager.WebUI.Components.Pages.AuthPage;

public partial class RegisterForm
{
    [Inject] private UserService UserService { get; set; } = null!;
    [Inject] private AuthService AuthService { get; set; } = null!;

    [Parameter] public Action<AuthPageCurrentState>? GoTo { get; set; }
    [Parameter] public Action<(string Email, Guid UserId, bool KeepSignedIn, string SenderEmail, int CodeLifetimeSeconds)>? UserChanged { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public bool IsUseOfTermsAgreed { get; set; }
    public bool IsConfirmPasswordValid { get; set; }
    public bool IsPasswordValid { get; set; }
    public bool IsEmailValid { get; set; }
    public bool IsUsernameValid { get; set; }

    private async Task OnRegisterClicked()
    {
        if (string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(ConfirmPassword) ||
            !IsUseOfTermsAgreed) return;

        await Loading(async () =>
        {
            var created = await UserService.Create(new CreateUserRequestDto
            {
                Id = Guid.NewGuid(),
                UserName = Username,
                Email = Email,
                Password = Password
            });

            if (!created) return;

            var twoFactor = await AuthService.Login(new LoginUser
            {
                LoginParameter = Email,
                Password = Password
            });

            if (twoFactor is null) return;

            UserChanged?.Invoke((twoFactor.MaskedEmail ?? Email, twoFactor.UserId, true, twoFactor.SenderEmail ?? string.Empty, twoFactor.CodeLifetimeSeconds));
            GoTo?.Invoke(AuthPageCurrentState.TwoFA);
        });
    }

    private void OnSignInClicked()
    {
        GoTo?.Invoke(AuthPageCurrentState.Login);
    }
}
