using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Components.Modals;
using ToDoTimeManager.WebUI.Services.Helpers.Modal;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Services.Interfaces;

namespace ToDoTimeManager.WebUI.Components.Shared;

public partial class NavMenu : IAsyncDisposable
{
    [Inject] private UserService UserService { get; set; } = null!;
    [Inject] private StatisticService StatisticService { get; set; } = null!;
    [Inject] private ISignalRService SignalRService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private IModalService ModalService { get; set; } = null!;

    private NavBarUserModel?   _navBarUser;
    private NavBarCountsModel? _counts;
    private bool               _isDrawerOpen;
    private IDisposable?       _userUpdatedSubscription;
    private IDisposable?       _countsUpdatedSubscription;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var isAuth    = authState.User.Identity?.IsAuthenticated == true;
        if (!isAuth) return;

        _navBarUser = await UserService.GetNavBarInfo();
        _counts     = await StatisticService.GetNavBarCounts();

        _userUpdatedSubscription = SignalRService.On<NavBarUserModel>("UserUpdated", updated =>
        {
            _navBarUser = updated;
            InvokeAsync(StateHasChanged);
        });

        _countsUpdatedSubscription = SignalRService.On<NavBarCountsModel>("NavBarCountsUpdated", updated =>
        {
            _counts = updated;
            InvokeAsync(StateHasChanged);
        });
    }

    private void ToggleDrawer()
    {
        _isDrawerOpen = !_isDrawerOpen;
    }

    private void CloseDrawer()
    {
        _isDrawerOpen = false;
    }

    public async ValueTask DisposeAsync()
    {
        _userUpdatedSubscription?.Dispose();
        _countsUpdatedSubscription?.Dispose();
        await ValueTask.CompletedTask;
    }

    private async Task OnLogoutClicked()
    {
        var modalParameters = new ModalParameters();
        modalParameters.Add("MessageDetails", Localizer["Are you sure you want to logout?"].Value);
        var modal = ModalService.Show<ConfirmModal>(Localizer["Logout confirmation"].Value, modalParameters);
        var res = await modal.Result;
        if (res is true)
            await ((CustomAuthStateProvider)AuthenticationStateProvider).MarkUserAsLoggedOut();
    }
}
