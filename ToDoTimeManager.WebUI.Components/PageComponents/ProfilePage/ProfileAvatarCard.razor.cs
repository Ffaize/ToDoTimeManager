using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ToDoTimeManager.Shared.DTOs.User;
using ToDoTimeManager.WebUI.Services.HttpServices;

namespace ToDoTimeManager.WebUI.Components.PageComponents.ProfilePage;

public partial class ProfileAvatarCard
{
    [Inject] private UserService UserService { get; set; } = null!;

    [Parameter, EditorRequired] public UserResponseDto  User            { get; set; } = null!;
    [Parameter]                 public EventCallback<string> OnAvatarChanged { get; set; }

    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    private string GetInitial() =>
        User.UserName?.Length > 0 ? User.UserName[..1].ToUpperInvariant() : "?";

    private async Task OnFileSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file.Size > MaxFileSizeBytes) return;

        await Loading(async () =>
        {
            var bytes = new byte[file.Size];
            await using var stream = file.OpenReadStream(MaxFileSizeBytes);
            _ = await stream.ReadAsync(bytes);

            var dataUri = $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
            var success = await UserService.UpdateAvatar(dataUri);
            if (success)
                await OnAvatarChanged.InvokeAsync(dataUri);
        });
    }
}
