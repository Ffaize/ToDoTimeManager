namespace ToDoTimeManager.WebUI.Services.Interfaces;

public interface ITwoFaTimerService
{
    void StartTimer(Guid userId);
    int GetRemainingSeconds(Guid userId);
    void ClearTimer(Guid userId);
}
