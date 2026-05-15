using ToDoTimeManager.WebUI.Services.Services.Implementations;

namespace ToDoTimeManager.WebUI.Services.Services.Interfaces;

public interface ITwoFaTimerService
{
    void StartTimer(Guid userId, int periodSeconds);
    TwoFaTimer? GetTimer(Guid userId);
    bool HasActiveTimer(Guid userId);
    void RemoveTimer(Guid userId);
}
