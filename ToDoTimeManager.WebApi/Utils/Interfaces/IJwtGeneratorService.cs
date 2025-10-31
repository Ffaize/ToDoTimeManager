using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.WebApi.Utils.Interfaces
{
    public interface IJwtGeneratorService
    {
        string GenerateAccessToken(string userId, UserRole? Role);
        string GenerateRefreshToken();
    }
}