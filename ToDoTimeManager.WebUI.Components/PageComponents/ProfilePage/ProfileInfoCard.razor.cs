using Microsoft.AspNetCore.Components;
using ToDoTimeManager.Shared.DTOs.User;
using ToDoTimeManager.WebUI.Services.HttpServices;

namespace ToDoTimeManager.WebUI.Components.PageComponents.ProfilePage;

public partial class ProfileInfoCard
{
    [Inject] private UserService UserService { get; set; } = null!;

    [Parameter, EditorRequired] public UserResponseDto              User          { get; set; } = null!;
    [Parameter]                 public EventCallback<UserResponseDto> OnUserUpdated { get; set; }

    private string? _userName;
    private string? _email;
    private string? _password;

    protected override void OnParametersSet()
    {
        _userName = User.UserName;
        _email    = User.Email;
        _password = string.Empty;
    }

    private async Task OnSaveClicked()
    {
        await Loading(async () =>
        {
            var request = new UpdateUserRequestDto
            {
                Id       = User.Id,
                UserName = _userName ?? string.Empty,
                Email    = _email    ?? string.Empty,
                Password = string.IsNullOrWhiteSpace(_password) ? null : _password
            };

            var success = await UserService.Update(request);
            if (!success) return;

            var updated = await UserService.GetUserById(User.Id);
            if (updated is not null)
                await OnUserUpdated.InvokeAsync(updated);

            _password = string.Empty;
        });
    }
}
