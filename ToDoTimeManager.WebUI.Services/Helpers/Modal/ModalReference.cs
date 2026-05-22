namespace ToDoTimeManager.WebUI.Services.Helpers.Modal;

public class ModalReference
{
    private readonly TaskCompletionSource<object> _tcs = new();

    public Task<object> Result => _tcs.Task;

    internal void SetResult(object result) => _tcs.TrySetResult(result);
}
