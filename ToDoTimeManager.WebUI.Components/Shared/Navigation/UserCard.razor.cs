using Microsoft.AspNetCore.Components;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebUI.Components.Shared.Navigation;

public partial class UserCard
{
    [Parameter] public NavBarUserModel? User { get; set; }
    [Parameter] public bool ShowRole { get; set; }
    [Parameter] public string AdditionalCssClass { get; set; } = string.Empty;

    private string GetDisplayName() => !string.IsNullOrWhiteSpace(User?.Name)
        ? User.Name
        : User?.Username ?? string.Empty;

    private string GetSecondaryLine() => ShowRole
        ? $"@{User?.Username} · {User?.Role}"
        : $"@{User?.Username}";

    private string GetInitial() => User?.Name?.Length > 0
        ? User.Name[..1].ToUpperInvariant()
        : "?";
}
