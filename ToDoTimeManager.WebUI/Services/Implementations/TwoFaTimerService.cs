using System.Collections.Concurrent;
using ToDoTimeManager.WebUI.Services.Interfaces;

namespace ToDoTimeManager.WebUI.Services.Implementations;

public class TwoFaTimerService : ITwoFaTimerService
{
    private const int CodeLifetimeSeconds = 300;
    private readonly ConcurrentDictionary<Guid, DateTime> _timers = new();

    public void StartTimer(Guid userId) => _timers[userId] = DateTime.UtcNow;

    public int GetRemainingSeconds(Guid userId)
    {
        if (!_timers.TryGetValue(userId, out var sentAt)) return 0;
        var elapsed = (DateTime.UtcNow - sentAt).TotalSeconds;
        return Math.Max(0, (int)(CodeLifetimeSeconds - elapsed));
    }

    public void ClearTimer(Guid userId) => _timers.TryRemove(userId, out _);
}
