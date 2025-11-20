using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net;
using System.Net.Http.Headers;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Services.CircuitServicesAccesor;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;
using ToDoTimeManager.WebUI.Utils;

namespace ToDoTimeManager.WebUI.Handlers;

public class TokenMessageHandler : DelegatingHandler
{
    private readonly CircuitServicesAccesor _circuitServicesAccesor;
    private readonly CustomAuthStateProvider? _authenticationStateProvider;

    public TokenMessageHandler(CircuitServicesAccesor circuitServicesAccesor, AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider as CustomAuthStateProvider;
        _circuitServicesAccesor = circuitServicesAccesor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var tokens = await ConfigureRequest(request);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized || tokens == null) return response;
            var refreshExpiry = tokens.RefreshTokenExpiresAt;
            if (refreshExpiry < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token expired");

            var authService = _circuitServicesAccesor.Service?.GetRequiredService<AuthService>();
            var tokenModel = await authService?.RefreshToken(tokens)!;
            if (tokenModel != null) await _authenticationStateProvider?.MarkUserAsAuthenticated(tokenModel)!;
            await ConfigureRequest(request);

            response.Dispose();
            response = await base.SendAsync(request, cancellationToken);

            return response;
        }
        catch (Exception)
        {
            await _authenticationStateProvider?.MarkUserAsLoggedOut()!;
            throw;
        }
    }

    private async Task<TokenModel?> ConfigureRequest(HttpRequestMessage request)
    {
        var localStorage = _circuitServicesAccesor?.Service?.GetRequiredService<ProtectedLocalStorage>();
        if (localStorage == null) return null;
        var tokens = await localStorage.GetTokenAsync();
        if (tokens is null) return null;
        var token = tokens.AccessToken;
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return tokens;

    }
}