using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace ToDoTimeManager.WebUI.Components.Shared.Navigation;

public partial class TopBar : IDisposable
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    public string SearchValue { get; set; } = string.Empty;

    private static readonly Dictionary<string, string> RouteLabels = new()
    {
        ["/"] = "Dashboard",
        ["/tasks"] = "Tasks",
        ["/time-logs"] = "TimeLogs",
        ["/projects"] = "Projects",
        ["/profile"] = "Profile",
        ["/settings"] = "Settings",
        ["/security"] = "Security",
    };

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    private string GetCurrentLabel()
    {
        var relativePath = "/" + NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('?')[0].Split('#')[0];
        relativePath = relativePath.TrimEnd('/');
        if (relativePath.Length == 0) relativePath = "/";

        var key = RouteLabels.GetValueOrDefault(relativePath, "Dashboard");
        return Localizer[key].Value;
    }

    private Task OnSearchKeyDown(KeyboardEventArgs e)
    {
        // TODO: implement global search navigation
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}
