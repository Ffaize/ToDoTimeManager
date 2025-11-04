using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces
{
    public interface IAuthService
    {
        TokenModel? AuthenticateUser(LoginUser loginUser, User user);
        TokenModel? RefreshAuthToken(TokenModel tokenModel);
    }
}
