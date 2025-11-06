namespace ToDoTimeManager.WebUI.Services.Interfaces;

public interface IToastMessagesService
{
    Task ShowToast(string message, bool isError = false);
    IAsyncEnumerable<(string message, bool isError)> GetToastStream(CancellationToken ct);
}