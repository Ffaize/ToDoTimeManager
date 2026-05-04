using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ToDoTimeManager.Shared.Utils;

public static class JwtTokenHelper
{
    public static (string? UserId, string? Role) GetUserDataFromAccessToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        return (userId, role);
    }

    public static ClaimsPrincipal GetClaimsPrincipal(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);
        return new ClaimsPrincipal(new ClaimsIdentity(token.Claims, "jwt"));
    }
}