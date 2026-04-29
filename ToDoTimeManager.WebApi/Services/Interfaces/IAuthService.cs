using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IAuthService
{
    Task<TwoFactorPendingModel?> Login(LoginUser loginUser);
    TokenModel? RefreshAuthToken(TokenModel tokenModel);
}