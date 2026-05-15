using Microsoft.AspNetCore.Components;

namespace ToDoTimeManager.WebUI.Components.StaticComponents.Toast;

public partial class ToastBox : IDisposable
{
    [Inject] private IToastsService ToastService { get; set; } = null!;

    protected override void OnInitialized()
    {
        ToastService.OnChange += OnChange;
        base.OnInitialized();
    }

    private async void OnChange()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        ToastService.OnChange -= OnChange;
    }
}