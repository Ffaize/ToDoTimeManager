using System.Timers;
using ToDoTimeManager.WebUI.Models.Enums;
using Timer = System.Timers.Timer;

namespace ToDoTimeManager.WebUI.Models.Models;

public class ToastModel : IDisposable
{
    public Guid Id { get; set; }
    public string? Message { get; set; }
    public ToastType ToastType { get; set; }
    public Timer? Timer { get; set; }
    public string? AnimationClass { get; set; }
    public string? AdditionalClass { get; set; }

    private Action? _notifyChange;
    private Action<ToastModel>? _removeSelf;

    public async Task Init(string message, ToastType toastType, Action notifyChange, Action<ToastModel> removeSelf)
    {
        Id = Guid.NewGuid();
        Message = message;
        ToastType = toastType;
        _notifyChange = notifyChange;
        _removeSelf = removeSelf;

        AnimationClass = "toast-fade-in";
        _notifyChange.Invoke();
        await Task.Delay(300);
        AnimationClass = string.Empty;
        AdditionalClass = "toast-faded";
        _notifyChange.Invoke();

        CreateTimer();
        Timer?.Start();
    }

    private void CreateTimer()
    {
        Timer = new Timer();
        Timer.AutoReset = false;
        Timer.Enabled = true;
        Timer.Interval = 4000;
        Timer.Elapsed += TimerOnElapsed;
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        _ = RemoveToast();
    }

    public async Task RemoveToast()
    {
        AdditionalClass = string.Empty;
        AnimationClass = "toast-fade-out";
        _notifyChange?.Invoke();
        await Task.Delay(300);

        AnimationClass = string.Empty;
        _removeSelf?.Invoke(this);
        _notifyChange?.Invoke();
        Dispose();
    }

    public void Dispose()
    {
        Timer?.Elapsed -= TimerOnElapsed;
        Timer?.Stop();
        Timer?.Dispose();
    }
}
