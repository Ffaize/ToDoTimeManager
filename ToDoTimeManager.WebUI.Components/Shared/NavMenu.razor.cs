using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Components.Modals;
using ToDoTimeManager.WebUI.Services.Helpers.Modal;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Services.Interfaces;

namespace ToDoTimeManager.WebUI.Components.Shared;

public partial class NavMenu : IAsyncDisposable
{
    private bool IsOpenProfileDropdown { get; set; }
    [Inject] private UserService UserService { get; set; } = null!;
    [Inject] private ISignalRService SignalRService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private IModalService ModalService { get; set; } = null!;

    public string SearchToDoValue { get; set; } = string.Empty;

    private NavBarUserModel? _navBarUser;
    private IDisposable?     _signalRSubscription;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var isAuth    = authState.User.Identity?.IsAuthenticated == true;
        if (!isAuth) return;

        _navBarUser          = await UserService.GetNavBarInfo();
        _signalRSubscription = SignalRService.On<NavBarUserModel>("UserUpdated", updated =>
        {
            _navBarUser = updated;
            InvokeAsync(StateHasChanged);
        });
    }

    private string GetDisplayName()  => !string.IsNullOrWhiteSpace(_navBarUser?.Name)
                                            ? _navBarUser.Name
                                            : _navBarUser?.Username ?? string.Empty;
    private string GetDisplayUserNameRole()  => "@" + _navBarUser?.Username + " · " + _navBarUser?.Role;
    private string GetInitial()      => _navBarUser?.Name?.Length > 0
                                            ? _navBarUser.Name[..1].ToUpperInvariant()
                                            : "?";

    private Task OnEnterPressed()
    {
        // TODO: implement global search navigation
        return Task.CompletedTask;
    }

    private void SwitchProfileDropdownState()
    {
        IsOpenProfileDropdown = !IsOpenProfileDropdown;
        InvokeAsync(StateHasChanged);
    }

    private Task OnProfileClicked()
    {
        IsOpenProfileDropdown = false;
        NavigationManager.NavigateTo("/profile");
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        _signalRSubscription?.Dispose();
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
