using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Services.Implementations;

namespace ToDoTimeManager.WebUI.Components.Toast;

public partial class ToastBox : IDisposable
{
    [Inject] private ToastsService ToastService { get; set; } = null!;

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