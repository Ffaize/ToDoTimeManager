using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenModel> AuthenticateUser(LoginUser loginUser);
        Task<TokenModel> RefreshAuthToken(TokenModel tokenModel);
    }
}
