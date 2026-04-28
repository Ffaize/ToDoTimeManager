using ToDoTimeManager.WebUI.Models;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.Interfaces;

namespace ToDoTimeManager.WebUI.Services.Implementations;

public class ToastsService : IToastsService
{
    public List<ToastModel> Messages { get; set; } = [];
    public Action? OnChange { get; set; }
    public List<ToastModel> GetMessages()
    {
        return Messages;
    }

    public async Task ShowToast(string message, ToastType toastType)
    {
        if (Messages is { Count: > 4 })
        {
            var firstToast = Messages.First();
            await firstToast.RemoveToast();
        }

        var toast = new ToastModel();
        Messages.Add(toast);
        await toast.Init(message, toastType, this);
        OnChange?.Invoke();
    }
}