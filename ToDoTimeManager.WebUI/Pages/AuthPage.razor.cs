using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using ToDoTimeManager.Shared.DTOs.User;
using ToDoTimeManager.WebUI.Models.Models;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Services.Implementations;
using ToDoTimeManager.WebUI.Services.Services.Interfaces;

using ToDoTimeManager.WebUI.Utils.PotectedLocalStorageHelpers;

namespace ToDoTimeManager.WebUI.Pages;

public partial class AuthPage
{
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private UserService UserService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private IToastsService ToastsService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILogger<AuthPage> Logger { get; set; } = null!;
    [Inject] private IModalService ModalService { get; set; } = null!;
    [Inject] private ITwoFaTimerService TwoFaTimerService { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;

    private static readonly AuthPageCurrentState[] NavOrder =
    [
        AuthPageCurrentState.Login,
        AuthPageCurrentState.Registration,
        AuthPageCurrentState.TwoFA,
        AuthPageCurrentState.ForgotPassword,
        AuthPageCurrentState.ResetPassword,
    ];

    protected const string SignInStepName = "sign-in";
    protected const string RegisterStepName = "register";
    protected const string TwoFaStepName = "two-fa";

    private bool _isAnimating;
    private AuthPageCurrentState _activeState = AuthPageCurrentState.Login;
    private AuthPageCurrentState? _exitingState;
    private string _activeClass = "auth-form-slide--active";
    private string _exitingClass = string.Empty;

    private PendingTwoFaSessionState _session = new();
    private UserResponseDto _user = new();
    private PendingPasswordResetState _passwordResetSession = new();
    private string _forgotPasswordInitialEmail = string.Empty;

    protected async Task GoTo(AuthPageCurrentState target)
    {
        if (_isAnimating) return;
        var current = _activeState;
        if (current == target) return;

        _isAnimating = true;
        if (target == AuthPageCurrentState.TwoFA)
            _session.SourceState = current;

        var isForward = Array.IndexOf(NavOrder, target) > Array.IndexOf(NavOrder, current);

        _exitingState = current;
        _exitingClass = isForward ? "auth-form-slide--exiting-left" : "auth-form-slide--exiting-right";
        _activeState = target;
        _activeClass = isForward ? "auth-form-slide--entering-right" : "auth-form-slide--entering-left";

        await InvokeAsync(StateHasChanged);
        await Task.Delay(450);

        _exitingState = null;
        _activeClass = "auth-form-slide--active";
        _isAnimating = false;
        await InvokeAsync(StateHasChanged);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            NavigationManager.NavigateTo(NavigationManager.BaseUri);
            return;
        }

        var uri = new Uri(NavigationManager.Uri);
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
        var googleSession = query.TryGetValue("google_session", out var val) ? val.ToString() : null;
        if (!string.IsNullOrWhiteSpace(googleSession))
        {
            await HandleGoogleSessionAsync(googleSession);
            return;
        }

        var githubSession = query.TryGetValue("github_session", out var ghVal) ? ghVal.ToString() : null;
        if (!string.IsNullOrWhiteSpace(githubSession))
        {
            await HandleGitHubSessionAsync(githubSession);
            return;
        }

        var pendingUser = await ProtectedLocalStorage.GetUserInfoAsync();
        if (pendingUser is null || pendingUser.Id == Guid.Empty) return;
        _user = pendingUser;

        if (!TwoFaTimerService.HasActiveTimer(pendingUser.Id))
        {
            await ProtectedLocalStorage.RemovePendingTwoFaContextAsync();
            return;
        }

        var pendingSessionState = await ProtectedLocalStorage.GetPendingTwoFaSessionStateAsync();
        _session = pendingSessionState ?? new PendingTwoFaSessionState { SourceState = AuthPageCurrentState.Login };

        _activeState = AuthPageCurrentState.TwoFA;
        await InvokeAsync(StateHasChanged);
    }

    private async Task HandleGoogleSessionAsync(string code)
        => await HandleOAuthSessionAsync(code, isGoogle: true);

    private async Task HandleGitHubSessionAsync(string code)
        => await HandleOAuthSessionAsync(code, isGoogle: false);

    private async Task HandleOAuthSessionAsync(string code, bool isGoogle)
    {
        var result = isGoogle
            ? await AuthService.ExchangeGoogleSessionAsync(code)
            : await AuthService.ExchangeGitHubSessionAsync(code);

        if (result == null)
        {
            NavigationManager.NavigateTo("/auth", forceLoad: true);
            return;
        }

        if (result.Token != null)
        {
            if (AuthenticationStateProvider is CustomAuthStateProvider authProvider)
                await authProvider.MarkUserAsAuthenticated(result.Token);

            NavigationManager.NavigateTo(NavigationManager.BaseUri);
            return;
        }

        if (result.PendingTwoFactor != null)
        {
            var pending = result.PendingTwoFactor;
            _user = new UserResponseDto { Id = pending.UserId, Email = pending.Email };
            _session = new PendingTwoFaSessionState
            {
                MaskedEmail = pending.Email ?? string.Empty,
                SenderEmail = pending.SenderEmail ?? string.Empty,
                CodeLifetimeSeconds = pending.CodeLifetimeSeconds,
                SourceState = AuthPageCurrentState.Login
            };
            TwoFaTimerService.StartTimer(pending.UserId, pending.CodeLifetimeSeconds);
            _activeState = AuthPageCurrentState.TwoFA;
            await InvokeAsync(StateHasChanged);
            return;
        }

        NavigationManager.NavigateTo("/auth", forceLoad: true);
    }

    protected void AuthInfoChanged(PendingTwoFaSessionState session, UserResponseDto user)
    {
        TwoFaTimerService.StartTimer(user.Id, session.CodeLifetimeSeconds);
        _session = session;
        _user = user;
    }

    protected void SetForgotPasswordInitialEmail(string loginParameter)
    {
        _forgotPasswordInitialEmail = loginParameter;
    }

    protected void PasswordResetInfoChanged(PendingPasswordResetState state)
    {
        if (state.UserId != Guid.Empty)
            TwoFaTimerService.StartTimer(state.UserId, state.CodeLifetimeSeconds);
        _passwordResetSession = state;
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key != "Enter") return;
        var task = _activeState switch
        {
            AuthPageCurrentState.Login => _loginFormRef?.InvokePrimaryAsync(),
            AuthPageCurrentState.Registration => _registerFormRef?.InvokePrimaryAsync(),
            AuthPageCurrentState.TwoFA => _twoFaFormRef?.InvokePrimaryAsync(),
            AuthPageCurrentState.ForgotPassword => _forgotPasswordFormRef?.InvokePrimaryAsync(),
            AuthPageCurrentState.ResetPassword => _resetPasswordFormRef?.InvokePrimaryAsync(),
            _ => null
        };
        if (task != null) await task;
    }
}
