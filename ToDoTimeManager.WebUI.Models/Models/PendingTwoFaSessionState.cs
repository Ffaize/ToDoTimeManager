using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.WebUI.Models.Enums;

namespace ToDoTimeManager.WebUI.Models.Models;

public class PendingTwoFaSessionState
{
    public PendingTwoFaSessionState(
        string maskedEmail,
        string senderEmail,
        bool keepSignedIn,
        int codeLifetimeSeconds,
        AuthPageCurrentState sourceState,
        TwoFactorMethod twoFactorMethod = TwoFactorMethod.Email)
    {
        MaskedEmail         = maskedEmail;
        SenderEmail         = senderEmail;
        KeepSignedIn        = keepSignedIn;
        CodeLifetimeSeconds = codeLifetimeSeconds;
        SourceState         = sourceState;
        TwoFactorMethod     = twoFactorMethod;
    }

    public PendingTwoFaSessionState() { }

    public string               MaskedEmail         { get; set; } = string.Empty;
    public string               SenderEmail         { get; set; } = string.Empty;
    public bool                 KeepSignedIn        { get; set; }
    public int                  CodeLifetimeSeconds { get; set; }
    public AuthPageCurrentState SourceState         { get; set; }
    public TwoFactorMethod      TwoFactorMethod     { get; set; } = TwoFactorMethod.Email;
}
