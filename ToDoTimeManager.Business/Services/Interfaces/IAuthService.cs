using System.Security.Claims;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> Login(LoginUser loginUser);
    Task<TokenModel?> RefreshAuthToken(TokenModel tokenModel);
    Task<TokenModel?> GetOrCreateGoogleUserTokenAsync(ClaimsPrincipal googleUser);
}