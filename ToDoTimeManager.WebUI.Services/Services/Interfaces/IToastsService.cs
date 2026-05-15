using ToDoTimeManager.WebUI.Models.Models;
using ToDoTimeManager.WebUI.Models.Enums;

namespace ToDoTimeManager.WebUI.Services.Services.Interfaces;

public interface IToastsService
{
    List<ToastModel> Messages { get; set; }
    Task ShowToast(string message, ToastType toastType);
    Action? OnChange { get; set; }
}
