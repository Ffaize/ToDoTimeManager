using Microsoft.AspNetCore.Components;

namespace ToDoTimeManager.WebUI.Components.PageComponents.AuthPage.Forms;

public partial class RegisterForm
{
    [Inject] private UserService UserService { get; set; } = null!;
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
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
                Password = Password,
                KeepSignedIn = true
            });

            if (loginResult?.Token is null) return;

            if (AuthenticationStateProvider is CustomAuthStateProvider authProvider)
                await authProvider.MarkUserAsAuthenticated(loginResult.Token);
            NavigationManager.NavigateTo(NavigationManager.BaseUri);
        });
    }

    private async Task OnSignInClicked()
    {
        if (GoTo != null) await GoTo(AuthPageCurrentState.Login);
    }
}
