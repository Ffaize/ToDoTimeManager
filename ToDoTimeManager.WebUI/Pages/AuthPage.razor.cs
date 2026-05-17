using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ToDoTimeManager.Shared.DTOs.User;
using ToDoTimeManager.WebUI.Models.Models;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.HttpServices;
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

    protected void AuthInfoChanged(PendingTwoFaSessionState session, UserResponseDto user)
    {
        TwoFaTimerService.StartTimer(user.Id, session.CodeLifetimeSeconds);
        _session = session;
        _user = user;
    }
}
