using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.Shared.Utils;
using ToDoTimeManager.WebUI.Components.Modals;
using ToDoTimeManager.WebUI.Localization;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;
using ToDoTimeManager.WebUI.Utils;

namespace ToDoTimeManager.WebUI.Pages;

public partial class ProfilePage
{

    [Inject] private UserService UserService { get; set; } = null!;
    [Inject] private ToastsService ToastsService { get; set; } = null!;
    [Inject] private StatisticService StatisticService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;
    [Inject] private ILogger<ProfilePage> Logger { get; set; } = null!;

    private CustomAuthStateProvider AuthStateProvider => (CustomAuthStateProvider)AuthenticationStateProvider;

    private User? CurrentUser { get; set; } = new() { Id = Guid.Empty };
    private List<ToDoCountStatisticsOfAllTime> ToDoStatistic { get; set; } = [];
    public bool IsButtonsDisabled => CurrentUser?.Id == Guid.Empty || IsLoading;
    public bool IsUserEditModalVisible { get; set; }
    public bool IsLogOutConfirmationVisible { get; set; }
    public bool IsDeleteConfirmationVisible { get; set; }


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
            await FetchData();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task FetchData()
    {
        ShowLoader();
        var userIdAndRoleAsync = await AuthStateProvider.GetUserIdAndRoleAsync();

        if (userIdAndRoleAsync != null)
        {
            var user = await UserService.GetUserById(userIdAndRoleAsync.Value.Item1);
            if (user is not null)
                CurrentUser = user;
        }

        if (CurrentUser != null)
            ToDoStatistic = await StatisticService.GetToDoCountStatisticsOfAllTimeByUserId(CurrentUser.Id);
        HideLoader();
        StateHasChanged();
    }

    private string GetFirstLetters()
    {
        if (string.IsNullOrWhiteSpace(CurrentUser?.UserName))
            return string.Empty;
        var names = CurrentUser.UserName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return names.Length == 1 ? names[0][..1].ToUpper() : (names[0][..1] + names[1][..1]).ToUpper();
    }

    private void UserModalStateChanged(ModalResult res)
    {
        IsUserEditModalVisible = res.Show;
        StateHasChanged();
        if (res.Value is User user) _ = EditUser(user);
        StateHasChanged();
    }

    private async Task EditUser(User user)
    {
        ShowLoader();
        var update = await UserService.Update(user);
        if (update)
        {
            await ToastsService.ShowToast(Localizer["UserUpdated"], false);
            await FetchData();
        }
        HideLoader();
        StateHasChanged();
    }

    private void ShowModal(string nameOfBoolProp)
    {
        try
        {
            var prop = GetType().GetProperty(nameOfBoolProp);
            prop?.SetValue(this, true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
        }
    }

    private void LogOutModalStateChanged(ModalResult obj)
    {
        IsLogOutConfirmationVisible = obj.Show;
        StateHasChanged();
        if (obj.Value is true)
            _ = AuthStateProvider.MarkUserAsLoggedOut();
        InvokeAsync(StateHasChanged);
    }

    private void DeleteModalStateChanged(ModalResult obj)
    {
        IsDeleteConfirmationVisible = obj.Show;
        StateHasChanged();
        if (obj.Value is true)
            _ = DeleteUser();
        InvokeAsync(StateHasChanged);
    }

    private async Task DeleteUser()
    {
        ShowLoader();
        if (CurrentUser is null || CurrentUser.Id == Guid.Empty)
        {
            HideLoader();
            return;
        }
        var delete = await UserService.Delete(CurrentUser.Id);
        if (delete)
            await AuthStateProvider.MarkUserAsLoggedOut();
        HideLoader();
        StateHasChanged();
    }

    private int GetCountOfStatistic(ToDoStatus status)
    {
        var result = ToDoStatistic.FirstOrDefault(x => x.ToDoStatus == status);
        return result?.Count ?? 0;
    }
}