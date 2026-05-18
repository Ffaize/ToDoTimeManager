namespace ToDoTimeManager.Shared.Models;

public class LoginResponse
{
    public bool RequiresTwoFactor { get; set; }
    public TwoFactorPendingModel? TwoFaPending { get; set; }
    public TokenModel? Token { get; set; }
}
