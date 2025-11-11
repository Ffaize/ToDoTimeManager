using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;

namespace ToDoTimeManager.WebUI.Shared;

public partial class AuthComponent
{
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    private LoginUser User { get; set; } = new();

    private async Task Login()
    {
        var result = await AuthService.Login(User);
        if (result is not null)
        {
            if (AuthenticationStateProvider is CustomAuthStateProvider customAuthStateProvider)
            {
                await customAuthStateProvider.MarkUserAsAuthenticated(result);
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