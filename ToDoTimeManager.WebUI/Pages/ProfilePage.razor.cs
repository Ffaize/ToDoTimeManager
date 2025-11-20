using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.Shared.Utils;
using ToDoTimeManager.WebUI.Localization;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;
using ToDoTimeManager.WebUI.Utils;

namespace ToDoTimeManager.WebUI.Pages;

public partial class ProfilePage
{

    [Inject] private UserService UserService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;

    private CustomAuthStateProvider AuthStateProvider => (CustomAuthStateProvider)AuthenticationStateProvider;

    private User CurrentUser { get; set; } = new() {Id = Guid.Empty};

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


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await FetchUserData();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task FetchUserData()
    {
        ShowLoader();
        var accessToken = (await ProtectedLocalStorage.GetTokenAsync())?.AccessToken;
        if (accessToken is null)
            return;
        var (userId, role) =
                JwtTokenHelper.GetUserDataFromAccessToken(accessToken);

        if(userId is null)
            return;

        var user = await UserService.GetUserById(Guid.Parse(userId));
        if (user is not null)
            CurrentUser = user;
        HideLoader();
        StateHasChanged();
    }

    private string GetFirstLetters()
    {
        if (string.IsNullOrWhiteSpace(CurrentUser.UserName))
            return string.Empty;
        var names = CurrentUser.UserName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return names.Length == 1 ? names[0][..1].ToUpper() : (names[0][..1] + names[1][..1]).ToUpper();
    }
}