using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using ToDoTimeManager.Shared.DTOs.User;
using ToDoTimeManager.WebUI.Services.HttpServices;

namespace ToDoTimeManager.WebUI.Pages;

public partial class ProfilePage
{
    [Inject] private UserService                 UserService                 { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private NavigationManager           NavigationManager           { get; set; } = null!;

    private UserResponseDto? _user;
    private bool             _isLoading;

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        StateHasChanged();

        try
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var idClaim   = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (idClaim is null || !Guid.TryParse(idClaim, out var userId))
            {
                NavigationManager.NavigateTo("/auth");
                return;
            }

            _user = await UserService.GetUserById(userId);
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private void OnAvatarChanged(string newDataUri)
    {
        if (_user is not null)
            _user.Avatar = newDataUri;
        StateHasChanged();
    }

    private void OnUserUpdated(UserResponseDto updated)
    {
        _user = updated;
        StateHasChanged();
    }
}
