namespace ToDoTimeManager.WebUI.Services.Modal;

public class ModalReference
{
    private readonly TaskCompletionSource<bool> _tcs = new();

    public Task<bool> Result => _tcs.Task;

    internal void SetResult(bool confirmed) => _tcs.TrySetResult(confirmed);
}
