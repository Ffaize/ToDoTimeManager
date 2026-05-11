using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Interfaces;

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
    private Guid _userId;

    private readonly Dictionary<AuthPageCurrentState, string> _slideClasses = new()
    {
        { AuthPageCurrentState.Login,         "auth-form-slide--active"       },
        { AuthPageCurrentState.Registration,  "auth-form-slide--hidden-right" },
        { AuthPageCurrentState.TwoFA,         "auth-form-slide--hidden-right" },
    };

    protected async void GoTo(AuthPageCurrentState target)
    {
        if (_isAnimating) return;
        var current = this.AuthPageCurrentState;
        if (current == target) return;

        _isAnimating = true;
        var isForward = Array.IndexOf(NavOrder, target) > Array.IndexOf(NavOrder, current);

        _slideClasses[current] = isForward ? "auth-form-slide--exiting-left" : "auth-form-slide--exiting-right";
        _slideClasses[target] = isForward ? "auth-form-slide--entering-right" : "auth-form-slide--entering-left";
        this.AuthPageCurrentState = target;

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

    protected string GetSlideClass(AuthPageCurrentState state) =>
        $"auth-form-slide {_slideClasses[state]}";

    protected void UserChanged((string Email, Guid UserId) user)
    {
        _email = user.Email;
        _userId = user.UserId;
    }
}
