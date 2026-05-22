using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Services.Services.Interfaces;

namespace ToDoTimeManager.WebUI.Components.Modals;

public partial class ConfirmModal
{
    [Inject] private IModalService ModalService { get; set; } = null!;

    private void OnConfirm() => ModalService.Close(true);
    private void OnCancel()  => ModalService.Close(false);
    [Parameter] public string MessageDetails { get; set; } = string.Empty;
    [Parameter] public string ConfirmText    { get; set; } = "Confirm";
    [Parameter] public string CancelText     { get; set; } = "Cancel";
}
