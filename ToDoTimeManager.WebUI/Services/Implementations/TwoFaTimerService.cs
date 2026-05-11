using System.Collections.Concurrent;
using ToDoTimeManager.WebUI.Services.Interfaces;

namespace ToDoTimeManager.WebUI.Services.Implementations;

public class TwoFaTimerService : ITwoFaTimerService
{
    private readonly ConcurrentDictionary<Guid, (DateTime SentAt, int LifetimeSeconds)> _timers = new();

    public void StartTimer(Guid userId, int lifetimeSeconds) =>
        _timers[userId] = (DateTime.UtcNow, lifetimeSeconds);

    public int GetRemainingSeconds(Guid userId)
    {
        if (!_timers.TryGetValue(userId, out var entry)) return 0;
        var elapsed = (DateTime.UtcNow - entry.SentAt).TotalSeconds;
        return Math.Max(0, (int)(entry.LifetimeSeconds - elapsed));
    }

    public void ClearTimer(Guid userId) => _timers.TryRemove(userId, out _);
}
