using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IAuthService
{
    Task<TwoFactorPendingModel?> Login(LoginUser loginUser);
    Task<TokenModel?> RefreshAuthToken(TokenModel tokenModel);
}