namespace ToDoTimeManager.WebUI.Services.Modal;

public class ModalParameters
{
    private readonly Dictionary<string, object?> _params = new();

    public void Add(string name, object? value) => _params[name] = value;

    public T? Get<T>(string name) =>
        _params.TryGetValue(name, out var val) ? (T?)val : default;

    internal IDictionary<string, object?> ToDictionary() =>
        _params.ToDictionary(p => p.Key, p => p.Value);
}
