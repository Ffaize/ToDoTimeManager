using System.Timers;
using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Services.Implementations;
using Timer = System.Timers.Timer;

namespace ToDoTimeManager.WebUI.Components.Toast;

public partial class Toast : IDisposable
{
    [Parameter] public ToastModel ToastModel { get; set; } = null!;
    [Inject] private ToastsService ToastService { get; set; } = null!;
    private Timer? _timer;

    protected override void OnInitialized()
    {
        _timer = new Timer();
        _timer?.Enabled = true;
        _timer?.AutoReset = false;
        _timer?.Interval = 3200000;
        _timer?.Elapsed += HideToast;
        base.OnInitialized();
    }

    private void HideToast(object? sender, ElapsedEventArgs e)
    {
        ToastService.Messages.Remove(ToastModel);
        ToastService.OnChange?.Invoke();
        Dispose();
    }


    public void Dispose()
    {
        if(_timer is null) return;
        _timer?.Stop();
        _timer?.Elapsed -= HideToast;
        _timer?.Dispose();
    }

    private void HideToastFromButton()
    {
        HideToast(null, null);
    }
}