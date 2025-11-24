using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Localization;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;

namespace ToDoTimeManager.WebUI.Pages;

public partial class AuthPage
{
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private UserService UserService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private ToastsService ToastsService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILogger<AuthPage> Logger { get; set; } = null!;
    private LoginUser LoginUser { get; set; } = new();
    private User RegisterUser { get; set; } = new() { Id = Guid.NewGuid(), UserRole = UserRole.User };
    private string ConfirmPassword { get; set; } = string.Empty;
    private bool IsLogin { get; set; } = true;
    protected Dictionary<string, string> PasswordInputsTypes { get; set; } = new();
    private CustomAuthStateProvider? CustomAuthStateProvider => AuthenticationStateProvider as CustomAuthStateProvider;
    public string LoginPasswordName => "LoginPassword";
    public string RegisterPasswordName => "RegisterPassword";
    public string ConfirmPasswordName => "ConfirmPassword";

    #region BaseForComponent
    [Inject] private IStringLocalizer<Resource> Localizer { get; set; } = null!;
    public bool IsLoading { get; set; }
    public void ShowLoader()
    {
        IsLoading = true;
        InvokeAsync(StateHasChanged);
    }
    public void HideLoader()
    {
        IsLoading = false;
        InvokeAsync(StateHasChanged);
    }
    #endregion

    protected override void OnInitialized()
    {
        PasswordInputsTypes.Add("LoginPassword", "password");
        PasswordInputsTypes.Add("RegisterPassword", "password");
        PasswordInputsTypes.Add("ConfirmPassword", "password");

        base.OnInitialized();
    }

    private async Task Login()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(LoginUser.LoginParameter) || string.IsNullOrWhiteSpace(LoginUser.Password))
            {
                await ToastsService.ShowToast(Localizer["PleaseEnterCredentials"], true);
                return;
            }
            ShowLoader();
            var result = await AuthService.Login(LoginUser);
            HideLoader();
            if (result is not null)
            {
                await CustomAuthStateProvider?.MarkUserAsAuthenticated(result)!;
                NavigationManager.NavigateTo(NavigationManager.BaseUri);
            }
            else
            {
                await CustomAuthStateProvider?.MarkUserAsLoggedOut()!;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
            await CustomAuthStateProvider?.MarkUserAsLoggedOut()!;
        }
    }

    private async Task Register()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(RegisterUser.Email) || string.IsNullOrWhiteSpace(RegisterUser.Password) || string.IsNullOrWhiteSpace(RegisterUser.UserName) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await ToastsService.ShowToast(Localizer["PleaseEnterAllCredentials"], true);
                return;
            }

            if (!RegisterUser.Password.Equals(ConfirmPassword))
            {

                await ToastsService.ShowToast(Localizer["PasswordsDontMatch"], true);
                return;
            }

            ShowLoader();
            var creationResult = await UserService.Create(RegisterUser);
            HideLoader();
            if (creationResult)
            {
                await ToastsService.ShowToast(Localizer["RegistrationSuccessful"], false);
                ChangeState();
            }
            else
                await ToastsService.ShowToast(Localizer["RegistrationFailed"], true);

        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
        }
    }

    private void TogglePasswordVisibility(string inputKey)
    {
        var item = PasswordInputsTypes[inputKey];
        PasswordInputsTypes[inputKey] = item == "password" ? "text" : "password";
        StateHasChanged();
    }

    private void ChangeState()
    {
        IsLogin = !IsLogin;
        StateHasChanged();
    }

    private static bool IsPasswordVisible(string passwordInputsType) => passwordInputsType == "text";
}