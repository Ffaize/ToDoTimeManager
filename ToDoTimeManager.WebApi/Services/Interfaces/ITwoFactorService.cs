using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface ITwoFactorService
{
    Task<TwoFactorPendingModel> SendCode(Guid userId);
    Task<TokenModel> VerifyCode(Guid userId, string code);
}
