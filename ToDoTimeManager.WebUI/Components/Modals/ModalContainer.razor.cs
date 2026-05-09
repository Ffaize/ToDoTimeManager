using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Services.Interfaces;

namespace ToDoTimeManager.WebUI.Components.Modals;

public partial class ModalContainer : IDisposable
{
    [Inject] private IModalService ModalService { get; set; } = null!;

    protected override void OnInitialized()
    {
        ModalService.OnChange += OnChange;
    }

    private async void OnChange() => await InvokeAsync(StateHasChanged);

    private void OnCancel() => ModalService.Close(false);

    private IDictionary<string, object?> GetParameters() =>
        ModalService.ActiveParameters?.ToDictionary() ?? new Dictionary<string, object?>();

    public void Dispose()
    {
        ModalService.OnChange -= OnChange;
    }
}
