using System.Security.Claims;
using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.WebApi.Utils.Interfaces
{
    public interface IJwtGeneratorService
    {
        public string GenerateAccessToken(string userId, UserRole? Role);
        public string GenerateRefreshToken();
        public (string? UserId, string? Role) GetUserDataFromAccessToken(string token);
    }
}