using System.Timers;
using Timer = System.Timers.Timer;

namespace ToDoTimeManager.WebUI.Services.Implementations;

public class ToastsService
{
    public List<ToastModel> Messages { get; set; } = [];
    public Action? OnChange { get; set; }
    public void ShowToast(string message, bool isError)
    {
        Messages.Add(new ToastModel(message, isError, this));
        OnChange?.Invoke();
    }
}


public class ToastModel : IDisposable
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public bool IsError { get; set; }
    public Timer Timer { get; set; }
    private ToastsService ToastsService { get; set; }
    public ToastModel(string message, bool isError, ToastsService toastsService)
    {
        Id = Guid.NewGuid();
        Message = message;
        IsError = isError;
        ToastsService = toastsService;
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
        RemoveToast();
    }

    public void RemoveToast()
    {
        ToastsService.Messages.Remove(this);
        ToastsService.OnChange?.Invoke();
        Dispose();
    }

    public void Dispose()
    {
        Timer.Elapsed -= TimerOnElapsed;
    }
}