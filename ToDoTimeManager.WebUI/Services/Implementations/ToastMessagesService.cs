using System.Runtime.CompilerServices;
using System.Threading.Channels;
using ToDoTimeManager.WebUI.Services.Interfaces;

namespace ToDoTimeManager.WebUI.Services.Implementations;

public class ToastMessagesService : IToastMessagesService
{
    private readonly Channel<(string message, bool isError)> _channel =
        Channel.CreateUnbounded<(string, bool)>();

    public Task ShowToast(string message, bool isError = false)
    {
        _channel.Writer.TryWrite((message, isError));
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<(string message, bool isError)> GetToastStream(
        [EnumeratorCancellation] CancellationToken ct)
    {
        while (await _channel.Reader.WaitToReadAsync(ct))
        {
            while (_channel.Reader.TryRead(out var item))
            {
                yield return item;
            }
        }
    }
}