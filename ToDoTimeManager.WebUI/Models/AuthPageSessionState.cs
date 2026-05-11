using ToDoTimeManager.WebUI.Models.Enums;

namespace ToDoTimeManager.WebUI.Models;

public record AuthPageSessionState(
    AuthPageCurrentState State,
    string Email,
    Guid UserId,
    bool KeepSignedIn,
    AuthPageCurrentState SourceState,
    string SenderEmail,
    int CodeLifetimeSeconds
);
