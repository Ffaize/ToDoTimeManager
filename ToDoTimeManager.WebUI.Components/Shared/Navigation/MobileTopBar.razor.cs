using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebUI.Components.Shared.Navigation;

public partial class MobileTopBar
{
    [Parameter] public NavBarCountsModel? Counts { get; set; }
    [Parameter] public EventCallback OnToggleDrawer { get; set; }

    public string SearchValue { get; set; } = string.Empty;

    private bool _isSearchOpen;

    private void ToggleSearch()
    {
        _isSearchOpen = !_isSearchOpen;
    }

    private Task OnSearchKeyDown(KeyboardEventArgs e)
    {
        // TODO: implement global search navigation
        return Task.CompletedTask;
    }
}
