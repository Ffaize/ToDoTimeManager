using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
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

    private string _email = string.Empty;
    private string _senderEmail = string.Empty;
    private Guid _userId;
    private bool _keepSignedIn = true;
    private AuthPageCurrentState _sourceState = AuthPageCurrentState.Login;

    private readonly Dictionary<AuthPageCurrentState, string> _slideClasses = new()
    {
        { AuthPageCurrentState.Login,         "auth-form-slide--active"       },
        { AuthPageCurrentState.Registration,  "auth-form-slide--hidden-right" },
        { AuthPageCurrentState.TwoFA,         "auth-form-slide--hidden-right" },
    };

    protected async Task GoTo(AuthPageCurrentState target)
    {
        if (_isAnimating) return;
        var current = this.AuthPageCurrentState;
        if (current == target) return;

        _isAnimating = true;
        if (target == AuthPageCurrentState.TwoFA)
            _sourceState = current;
        var isForward = Array.IndexOf(NavOrder, target) > Array.IndexOf(NavOrder, current);

        _slideClasses[current] = isForward ? "auth-form-slide--exiting-left" : "auth-form-slide--exiting-right";
        _slideClasses[target] = isForward ? "auth-form-slide--entering-right" : "auth-form-slide--entering-left";
        this.AuthPageCurrentState = target;

        await ProtectedLocalStorage.SaveAuthPageStateAsync(new AuthPageSessionState(
            target, _email, _userId, _keepSignedIn, _sourceState, _senderEmail));

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
            NavigationManager.NavigateTo("/dashboard");
            return;
        }

        var saved = await ProtectedLocalStorage.GetAuthPageStateAsync();
        if (saved is null) return;

        _email = saved.Email;
        _senderEmail = saved.SenderEmail;
        _userId = saved.UserId;
        _keepSignedIn = saved.KeepSignedIn;
        _sourceState = saved.SourceState;

        var targetIndex = Array.IndexOf(NavOrder, saved.State);
        foreach (var state in NavOrder)
        {
            var idx = Array.IndexOf(NavOrder, state);
            _slideClasses[state] = state == saved.State
                ? "auth-form-slide--active"
                : (idx < targetIndex ? "auth-form-slide--hidden-left" : "auth-form-slide--hidden-right");
        }
        AuthPageCurrentState = saved.State;
        await InvokeAsync(StateHasChanged);
    }

    protected string GetSlideClass(AuthPageCurrentState state) =>
        $"auth-form-slide {_slideClasses[state]}";

    protected void UserChanged((string Email, string SenderEmail, Guid UserId, bool KeepSignedIn) user)
    {
        _email = user.Email;
        _senderEmail = user.SenderEmail;
        _userId = user.UserId;
        _keepSignedIn = user.KeepSignedIn;
        TwoFaTimerService.StartTimer(user.UserId);
    }
}
