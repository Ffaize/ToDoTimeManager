using ToDoTimeManager.WebUI.Services.Implementations;

namespace ToDoTimeManager.WebUI.Services.Interfaces;

public interface ITwoFaTimerService
{
    void StartTimer(Guid userId, int periodSeconds);
    TwoFaTimer? GetTimer(Guid userId);
    bool HasActiveTimer(Guid userId);
    void RemoveTimer(Guid userId);
}
