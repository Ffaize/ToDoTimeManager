using ToDoTimeManager.WebUI.Models.Models;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.Services.Interfaces;

namespace ToDoTimeManager.WebUI.Services.Services.Implementations;

public class ToastsService : IToastsService
{
    public List<ToastModel> Messages { get; set; } = [];
    public Action? OnChange { get; set; }

    public async Task ShowToast(string message, ToastType toastType)
    {
        if (Messages is { Count: > 4 })
            await Messages.First().RemoveToast();

        var toast = new ToastModel();
        Messages.Add(toast);
        await toast.Init(message, toastType, () => OnChange?.Invoke(), m => Messages.Remove(m));
        OnChange?.Invoke();
    }
}
