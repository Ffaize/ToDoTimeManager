using ToDoTimeManager.Entities.Entities;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Interfaces;

public interface ITwoFactorService
{
    Task<TwoFactorPendingModel> SendCode(UserEntity user);
    Task<TokenModel> VerifyCode(Guid userId, string code, bool keepSignedIn);
}
