using System.Timers;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.Implementations;
using Timer = System.Timers.Timer;

namespace ToDoTimeManager.WebUI.Models;

public class ToastModel : IDisposable
{
    public Guid Id { get; set; }
    public string? Message { get; set; }
    public ToastType ToastType { get; set; }
    public Timer? Timer { get; set; }
    private ToastsService? ToastsService { get; set; }
    public string? AnimationClass { get; set; }
    public string? AdditionalClass { get; set; }

    public async Task Init(string message, ToastType toastType, ToastsService toastsService)
    {
        Id = Guid.NewGuid();
        Message = message;
        ToastType = toastType;
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
        Timer?.Stop();
        Timer?.Dispose();
    }
}