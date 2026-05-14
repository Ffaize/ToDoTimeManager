using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface ITwoFactorService
{
    Task<TwoFactorPendingModel> SendCode(UserEntity user);
    Task<TokenModel> VerifyCode(Guid userId, string code, bool keepSignedIn);
}
