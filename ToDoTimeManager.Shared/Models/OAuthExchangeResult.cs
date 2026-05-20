namespace ToDoTimeManager.Shared.Models;

public class OAuthExchangeResult
{
    public TokenModel? Token { get; set; }
    public TwoFactorPendingModel? PendingTwoFactor { get; set; }
}
