using ToDoTimeManager.WebUI.Models.Enums;

namespace ToDoTimeManager.WebUI.Models.Models;

public class PendingTwoFaSessionState
{
    public PendingTwoFaSessionState(
        string maskedEmail,
        string senderEmail,
        bool keepSignedIn,
        int codeLifetimeSeconds,
        AuthPageCurrentState sourceState)
    {
        MaskedEmail = maskedEmail;
        SenderEmail = senderEmail;
        KeepSignedIn = keepSignedIn;
        CodeLifetimeSeconds = codeLifetimeSeconds;
        SourceState = sourceState;
    }

    public PendingTwoFaSessionState()
    {
        
    }

    public string MaskedEmail { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public bool KeepSignedIn { get; set; }
    public int CodeLifetimeSeconds { get; set; }
    public AuthPageCurrentState SourceState { get; set; }
}

