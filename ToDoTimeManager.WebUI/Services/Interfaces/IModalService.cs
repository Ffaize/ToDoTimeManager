using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Services.Modal;

namespace ToDoTimeManager.WebUI.Services.Interfaces;

public interface IModalService
{
    ModalReference Show<TComponent>(string title, ModalParameters? parameters = null)
        where TComponent : IComponent;

    void Close(bool confirmed);

    event Action? OnChange;

    Type? ActiveComponentType { get; }
    string? ActiveTitle { get; }
    ModalParameters? ActiveParameters { get; }
}
