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

    [Parameter] public Func<AuthPageCurrentState, Task>? GoTo { get; set; }
    [Parameter] public Action<(string Email, string SenderEmail, Guid UserId, bool KeepSignedIn)>? UserChanged { get; set; }

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
        if (!IsUsernameValid || !IsEmailValid || !IsPasswordValid || !IsConfirmPasswordValid ||
            string.IsNullOrWhiteSpace(Username) ||
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

            UserChanged?.Invoke((twoFactor.MaskedEmail ?? Email, twoFactor.SenderEmail ?? string.Empty, twoFactor.UserId, true));
            if (GoTo != null) await GoTo(AuthPageCurrentState.TwoFA);
        });
    }

    private async Task OnSignInClicked()
    {
        if (GoTo != null) await GoTo(AuthPageCurrentState.Login);
    }
}
