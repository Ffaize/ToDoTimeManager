using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Business.Utils.Interfaces;

public interface IJwtGeneratorService
{
    string GenerateAccessToken(string userId, UserRole? Role);
    string GenerateRefreshToken();
}
