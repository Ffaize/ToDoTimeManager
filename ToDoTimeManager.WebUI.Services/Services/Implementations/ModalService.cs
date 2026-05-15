using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Services.Helpers.Modal;
using ToDoTimeManager.WebUI.Services.Services.Interfaces;

namespace ToDoTimeManager.WebUI.Services.Services.Implementations;

public class ModalService : IModalService
{
    public Type? ActiveComponentType { get; private set; }
    public string? ActiveTitle { get; private set; }
    public ModalParameters? ActiveParameters { get; private set; }

    private ModalReference? _current;

    public event Action? OnChange;

    public ModalReference Show<TComponent>(string title, ModalParameters? parameters = null)
        where TComponent : IComponent
    {
        ActiveComponentType = typeof(TComponent);
        ActiveTitle = title;
        ActiveParameters = parameters;
        _current = new ModalReference();
        OnChange?.Invoke();
        return _current;
    }

    public void Close(bool confirmed)
    {
        _current?.SetResult(confirmed);
        ActiveComponentType = null;
        ActiveTitle = null;
        ActiveParameters = null;
        _current = null;
        OnChange?.Invoke();
    }
}
