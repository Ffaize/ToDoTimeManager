using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Models;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Utils;

namespace ToDoTimeManager.WebUI.Components.Pages.AuthPage;

public partial class RegisterForm
{
    [Inject] private UserService UserService { get; set; } = null!;
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;

    [Parameter] public Func<AuthPageCurrentState, Task>? GoTo { get; set; }
    [Parameter] public Action<PendingTwoFaSessionState, UserResponseDto>? AuthInfoChanged { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public bool IsUseOfTermsAgreed { get; set; }
    public bool IsConfirmPasswordValid { get; set; }
    public bool IsPasswordValid { get; set; }
    public bool IsEmailValid { get; set; }
    public bool IsUsernameValid { get; set; }
    public bool IsButtonDisabled =>
        !IsConfirmPasswordValid || !IsPasswordValid || !IsEmailValid || !IsUsernameValid || !IsUseOfTermsAgreed || IsLoading;

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

            var loginResult = await AuthService.Login(new LoginUser
            {
                LoginParameter = Email,
                Password = Password
            });

            if (loginResult is null) return;

            var user = new UserResponseDto
            {
                Id = loginResult.UserId,
                Email = loginResult.Email ?? string.Empty,
                UserName = null
            };

            var session = new PendingTwoFaSessionState(
                loginResult.Email ?? string.Empty,
                loginResult.SenderEmail ?? string.Empty,
                true,
                loginResult.CodeLifetimeSeconds,
                AuthPageCurrentState.Login
            );

            await ProtectedLocalStorage.SaveUserInfoAsync(user);
            await ProtectedLocalStorage.SavePendingTwoFaSessionStateAsync(session);


            AuthInfoChanged?.Invoke(session, user);
            if (GoTo != null) await GoTo(AuthPageCurrentState.TwoFA);
        });
    }

    private async Task OnSignInClicked()
    {
        if (GoTo != null) await GoTo(AuthPageCurrentState.Login);
    }
}
