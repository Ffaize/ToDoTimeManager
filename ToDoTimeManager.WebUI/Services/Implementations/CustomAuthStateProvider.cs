using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Services.CircuitServicesAccesor;
using ToDoTimeManager.WebUI.Services.HttpServices;

namespace ToDoTimeManager.WebUI.Services.Implementations;

public class CustomAuthStateProvider(CircuitServicesAccesor.CircuitServicesAccesor circuitServicesAccesor) : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var localStorage = circuitServicesAccesor?.Service?.GetRequiredService<ProtectedLocalStorage>();
            if (localStorage == null) return new AuthenticationState(_anonymous);
            var result = await localStorage.GetAsync<TokenModel>(nameof(TokenModel));
            if (!result.Success || result.Value is null)
                return new AuthenticationState(_anonymous);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(result.Value.AccessToken) as JwtSecurityToken;

            if (jsonToken != null)
            {
                if (jsonToken.ValidTo < DateTime.UtcNow && result.Value.RefreshTokenExpiresAt > DateTime.UtcNow)
                {
                    var authService = circuitServicesAccesor?.Service?.GetRequiredService<AuthService>();
                    var res = await authService?.RefreshToken(result.Value)!;
                    await MarkUserAsAuthenticated(res);
                    if (res?.AccessToken != null)
                    {
                        var claimsPrincipals = GetClaimsPrincipalFromJwt(res.AccessToken);
                        return new AuthenticationState(claimsPrincipals);
                    }
                }

            }

            if (jsonToken == null)
            {
                await localStorage.DeleteAsync(nameof(TokenModel));
                return new AuthenticationState(_anonymous);
            }


            if (result.Value.AccessToken != null)
            {
                var claimsPrincipal = GetClaimsPrincipalFromJwt(result.Value.AccessToken);
                return new AuthenticationState(claimsPrincipal);
            }

            return new AuthenticationState(_anonymous);
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    private static ClaimsPrincipal GetClaimsPrincipalFromJwt(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);

        var identity = new ClaimsIdentity(token.Claims, "jwt");
        return new ClaimsPrincipal(identity);
    }

    public async Task MarkUserAsAuthenticated(TokenModel tokens)
    {
        var localStorage = circuitServicesAccesor?.Service?.GetRequiredService<ProtectedLocalStorage>();
        if (localStorage != null) await localStorage.SetAsync(nameof(TokenModel), tokens);

        if (tokens.AccessToken != null)
        {
            var principal = GetClaimsPrincipalFromJwt(tokens.AccessToken);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }
    }

    public async Task MarkUserAsLoggedOut()
    {
        var localStorage = circuitServicesAccesor?.Service?.GetRequiredService<ProtectedLocalStorage>();
        if (localStorage != null) await localStorage.DeleteAsync(nameof(TokenModel));
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}