using System.Collections.Concurrent;
using System.Diagnostics;
using ToDoTimeManager.WebUI.Services.Interfaces;
using Timer = System.Timers.Timer;

namespace ToDoTimeManager.WebUI.Services.Implementations;

public class TwoFaTimerService : ITwoFaTimerService
{
    private readonly ConcurrentDictionary<Guid, TwoFaTimer> _timers = new();

    public void StartTimer(Guid userId, int periodSeconds)
    {
        var timer = new TwoFaTimer
        {
            PeriodSeconds = periodSeconds,
            TwoFaTimerService = this,
            UserId = userId
        };
        _timers[userId] = timer;
        timer.StartTimer();
    }

    public TwoFaTimer? GetTimer(Guid userId)
    {
        return _timers.GetValueOrDefault(userId);
    }

    public bool HasActiveTimer(Guid userId)
    {
        return _timers.ContainsKey(userId);
    }

    public void RemoveTimer(Guid userId) => _timers.TryRemove(userId, out var _);
}

public class TwoFaTimer : IDisposable
{
    public int PeriodSeconds { get; set; }
    public Guid UserId { get; set; }
    public int RemainingSeconds { get; private set; }
    public Action<int>? OnRemainingSecondsChanged { get; set; }
    public required ITwoFaTimerService TwoFaTimerService { get; set; }

    private PeriodicTimer? _timer;

    public void StartTimer()
    {
        _ = StartCountdown();
    }

    private async Task StartCountdown()
    {
        RemainingSeconds = PeriodSeconds;
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        while (await _timer.WaitForNextTickAsync())
        {
            RemainingSeconds--;
            OnRemainingSecondsChanged?.Invoke(RemainingSeconds);
            if (RemainingSeconds > 0) continue;
            Dispose();
            break;
        }
    }

    public void Dispose()
    {
        TwoFaTimerService.RemoveTimer(UserId);
        _timer?.Dispose();
        OnRemainingSecondsChanged = null;
    }
}
