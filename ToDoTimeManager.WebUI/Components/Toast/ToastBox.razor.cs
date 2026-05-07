using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Services.Interfaces;

namespace ToDoTimeManager.WebUI.Components.Toast;

public partial class ToastBox : IDisposable
{
    [Inject] private IToastsService ToastService { get; set; } = null!;

    protected override void OnInitialized()
    {
        ToastService.OnChange += OnChange;
        base.OnInitialized();
    }

    private void OnChange()
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        ToastService.OnChange -= OnChange;
    }
}