using System.Timers;
using Timer = System.Timers.Timer;

namespace ToDoTimeManager.WebUI.Services.Implementations;

public class ToastsService
{
    public List<ToastModel> Messages { get; set; } = [];
    public Action? OnChange { get; set; }
    public async Task ShowToast(string message, bool isError)
    {
        if (Messages is { Count: > 4 })
        {
            var firstToast = Messages.First();
            await firstToast.RemoveToast();
        }

        var toast = new ToastModel();
        Messages.Add(toast);
        await toast.Init(message, isError, this);
        OnChange?.Invoke();
    }
}


public class ToastModel : IDisposable
{
    public Guid Id { get; set; }
    public string? Message { get; set; }
    public bool IsError { get; set; }
    public Timer? Timer { get; set; }
    private ToastsService? ToastsService { get; set; }
    public string? AnimationClass { get; set; }
    public string? AdditionalClass { get; set; }

    public async Task Init(string message, bool isError, ToastsService toastsService)
    {
        Id = Guid.NewGuid();
        Message = message;
        IsError = isError;
        ToastsService = toastsService;

        AnimationClass = "toast-fade-in";
        ToastsService.OnChange?.Invoke();
        await Task.Delay(300);
        AnimationClass = string.Empty;
        AdditionalClass = "toast-faded";
        ToastsService.OnChange?.Invoke();

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
        ToastsService?.OnChange?.Invoke();
        await Task.Delay(300);

        AnimationClass = string.Empty;
        ToastsService?.Messages.Remove(this);
        ToastsService?.OnChange?.Invoke();
        Dispose();
    }

    public void Dispose()
    {
        Timer?.Elapsed -= TimerOnElapsed;
    }
}