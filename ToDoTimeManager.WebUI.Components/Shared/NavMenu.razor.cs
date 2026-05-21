using Microsoft.AspNetCore.Components;

namespace ToDoTimeManager.WebUI.Components.Shared;

public partial class NavMenu
{
    private bool IsOpenProfileDropdown { get; set; }
    [Inject] private IStringLocalizer<Resource> Localizer { get; set; } = null!;
    public string SearchToDoValue { get; set; } = string.Empty;

    private Task OnEnterPressed()
    {
        throw new NotImplementedException();
    }

    private void SwitchProfileDropdownState()
    {
        IsOpenProfileDropdown = !IsOpenProfileDropdown;
        InvokeAsync(StateHasChanged);
    }

    private Task OnProfileClicked()
    {
        throw new NotImplementedException();
    }
}