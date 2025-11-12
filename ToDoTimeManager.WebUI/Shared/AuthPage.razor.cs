using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;

namespace ToDoTimeManager.WebUI.Shared;

public partial class AuthPage
{
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    private LoginUser User { get; set; } = new();

    private async Task Login()
    {
        var result = await AuthService.Login(User);
        if (result is not null)
        {
            if (AuthenticationStateProvider is CustomAuthStateProvider customAuthStateProvider)
            {
                await customAuthStateProvider.MarkUserAsAuthenticated(result);
                NavigationManager.NavigateTo(NavigationManager.BaseUri);
            }

        }
        else
        {
            if (AuthenticationStateProvider is CustomAuthStateProvider customAuthStateProvider)
            {
                await customAuthStateProvider.MarkUserAsLoggedOut();
            }
        }

    }
}