using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.WebUI.Models;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Interfaces;
using ToDoTimeManager.WebUI.Utils;

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

    private AuthPageCurrentState AuthPageCurrentState { get; set; } = AuthPageCurrentState.Login;
    protected const string SignInStepName = "sign-in";
    protected const string RegisterStepName = "register";
    protected const string TwoFaStepName = "two-fa";

    private bool _isAnimating = false;

    private PendingTwoFaSessionState _session = new();
    private UserResponseDto _user = new();

    private readonly Dictionary<AuthPageCurrentState, string> _slideClasses = new()
    {
        { AuthPageCurrentState.Login,         "auth-form-slide--active"       },
        { AuthPageCurrentState.Registration,  "auth-form-slide--hidden-right" },
        { AuthPageCurrentState.TwoFA,         "auth-form-slide--hidden-right" },
    };

    protected async Task GoTo(AuthPageCurrentState target)
    {
        if (_isAnimating) return;
        var current = AuthPageCurrentState;
        if (current == target) return;

        _isAnimating = true;
        if (target == AuthPageCurrentState.TwoFA)
            _session.SourceState = current;
        var isForward = Array.IndexOf(NavOrder, target) > Array.IndexOf(NavOrder, current);

        _slideClasses[current] = isForward ? "auth-form-slide--exiting-left" : "auth-form-slide--exiting-right";
        _slideClasses[target] = isForward ? "auth-form-slide--entering-right" : "auth-form-slide--entering-left";
        AuthPageCurrentState = target;

        await InvokeAsync(StateHasChanged);
        await Task.Delay(450);

        foreach (var state in NavOrder)
        {
            if (state == target) continue;
            var isOnRight = Array.IndexOf(NavOrder, state) > Array.IndexOf(NavOrder, target);
            _slideClasses[state] = isOnRight ? "auth-form-slide--hidden-right" : "auth-form-slide--hidden-left";
        }
        _slideClasses[target] = "auth-form-slide--active";

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


        foreach (var state in NavOrder)
        {
            _slideClasses[state] = state == AuthPageCurrentState.TwoFA
                ? "auth-form-slide--active"
                : (Array.IndexOf(NavOrder, state) < Array.IndexOf(NavOrder, AuthPageCurrentState.TwoFA)
                    ? "auth-form-slide--hidden-left"
                    : "auth-form-slide--hidden-right");
        }
        AuthPageCurrentState = AuthPageCurrentState.TwoFA;
        await InvokeAsync(StateHasChanged);
    }

    protected string GetSlideClass(AuthPageCurrentState state) =>
        $"auth-form-slide {_slideClasses[state]}";

    protected void AuthInfoChanged(PendingTwoFaSessionState session, UserResponseDto user)
    {
        TwoFaTimerService.StartTimer(user.Id, session.CodeLifetimeSeconds);
        _session = session;
        _user = user;
    }
}
