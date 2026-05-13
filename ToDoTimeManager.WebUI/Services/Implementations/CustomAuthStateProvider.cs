using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.Shared.Utils;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Utils;

namespace ToDoTimeManager.WebUI.Services.Implementations;

public class CustomAuthStateProvider(
    CircuitServicesAccesor.CircuitServicesAccesor circuitServicesAccesor,
    ILogger<CustomAuthStateProvider> logger)
    : AuthenticationStateProvider
{
    private static readonly JwtSecurityTokenHandler JwtHandler = new();
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var localStorage = circuitServicesAccesor?.Service?.GetRequiredService<ProtectedLocalStorage>();
            if (localStorage == null) return new AuthenticationState(_anonymous);

            var tokens = await localStorage.GetTokenAsync();
            if (tokens is null) return new AuthenticationState(_anonymous);

            var jsonToken = JwtHandler.ReadToken(tokens.AccessToken) as JwtSecurityToken;

            if (jsonToken == null)
            {
                await localStorage.RemoveTokenAsync();
                return new AuthenticationState(_anonymous);
            }

            // Access token is still valid — fast path.
            if (jsonToken.ValidTo >= DateTime.UtcNow)
                return new AuthenticationState(JwtTokenHelper.GetClaimsPrincipal(tokens.AccessToken!));

            // Access token expired — check whether the refresh token can save us.
            if (tokens.RefreshTokenExpiresAt == null || tokens.RefreshTokenExpiresAt <= DateTime.UtcNow)
            {
                logger.LogInformation("Both access and refresh tokens are expired. Clearing session.");
                await localStorage.RemoveTokenAsync();
                return new AuthenticationState(_anonymous);
            }

            // Attempt a token refresh, serialised through TokenRefreshService.
            var authService = circuitServicesAccesor?.Service?.GetRequiredService<AuthService>();
            if (authService == null)
            {
                logger.LogError("AuthService not available. Cannot refresh token.");
                return new AuthenticationState(_anonymous);
            }

            var refreshService = circuitServicesAccesor?.Service?.GetService<TokenRefreshService>();

            TokenModel? refreshed;
            if (refreshService != null)
            {
                refreshed = await refreshService.TryRefreshAsync(
                    tokens,
                    t => authService.RefreshToken(t),
                    () => localStorage.GetTokenAsync());
            }
            else
            {
                refreshed = await authService.RefreshToken(tokens);
            }

            if (refreshed?.AccessToken != null)
            {
                await MarkUserAsAuthenticated(refreshed);
                return new AuthenticationState(JwtTokenHelper.GetClaimsPrincipal(refreshed.AccessToken));
            }

            logger.LogWarning("Token refresh returned null. Clearing session.");
            await localStorage.RemoveTokenAsync();
            return new AuthenticationState(_anonymous);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in GetAuthenticationStateAsync.");
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task MarkUserAsAuthenticated(TokenModel tokens)
    {
        var localStorage = circuitServicesAccesor?.Service?.GetRequiredService<ProtectedLocalStorage>();
        if (localStorage != null) await localStorage.SaveTokenAsync(tokens);

        if (tokens.AccessToken != null)
        {
            var principal = JwtTokenHelper.GetClaimsPrincipal(tokens.AccessToken);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }
    }

    public async Task MarkUserAsLoggedOut()
    {
        var localStorage = circuitServicesAccesor?.Service?.GetRequiredService<ProtectedLocalStorage>();
        if (localStorage != null)
        {
            await localStorage.RemoveTokenAsync();
            await localStorage.RemovePendingTwoFaContextAsync();
        }
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    public async Task<(Guid, UserRole)?> GetUserIdAndRoleAsync()
    {
        var localStorage = circuitServicesAccesor?.Service?.GetRequiredService<ProtectedLocalStorage>();
        if (localStorage == null) return null;

        var tokens = await localStorage.GetTokenAsync();
        if (tokens?.AccessToken is null) return null;

        // Reject expired access tokens — callers should not act on stale identity data.
        if (JwtHandler.ReadToken(tokens.AccessToken) is JwtSecurityToken jwt && jwt.ValidTo < DateTime.UtcNow)
            return null;

        var (userId, role) = JwtTokenHelper.GetUserDataFromAccessToken(tokens.AccessToken);
        if (userId is null || role is null) return null;

        return (Guid.Parse(userId), Enum.Parse<UserRole>(role));
    }
}
