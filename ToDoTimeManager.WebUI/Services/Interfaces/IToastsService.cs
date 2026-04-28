using ToDoTimeManager.WebUI.Models;
using ToDoTimeManager.WebUI.Models.Enums;

namespace ToDoTimeManager.WebUI.Services.Interfaces;

public interface IToastsService
{
    public List<ToastModel> Messages { get; set; }
    public Task ShowToast(string message, ToastType toastType);
    public Action? OnChange { get; set; }
}