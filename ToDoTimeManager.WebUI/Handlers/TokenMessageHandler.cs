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
    private readonly CustomAuthStateProvider? _authStateProvider;
    private readonly ILogger<TokenMessageHandler> _logger;

    public TokenMessageHandler(
        CircuitServicesAccesor circuitServicesAccesor,
        AuthenticationStateProvider authenticationStateProvider,
        ILogger<TokenMessageHandler> logger)
    {
        _authStateProvider = authenticationStateProvider as CustomAuthStateProvider;
        _circuitServicesAccesor = circuitServicesAccesor;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Buffer request body before the first send so it can be replayed on retry.
        // Without this, POST/PUT/PATCH bodies are consumed and lost on the first attempt.
        await BufferRequestBodyAsync(request);

        try
        {
            var tokens = await AttachBearerTokenAsync(request);
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized || tokens == null)
                return response;

            if (tokens.RefreshTokenExpiresAt <= DateTime.UtcNow)
            {
                _logger.LogWarning("Received 401 and refresh token is already expired. Logging out.");
                await LogOutAsync();
                return response;
            }

            var refreshed = await RefreshTokenAsync(tokens, cancellationToken);
            if (refreshed == null)
            {
                _logger.LogWarning("Token refresh failed after 401. Logging out.");
                await LogOutAsync();
                return response;
            }

            if (_authStateProvider != null)
                await _authStateProvider.MarkUserAsAuthenticated(refreshed);

            // Replay the original request with the new access token.
            response.Dispose();
            await AttachBearerTokenAsync(request, refreshed);
            return await base.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in TokenMessageHandler.");
            throw;
        }
    }

    // Reads the entire body into a byte array so it survives multiple SendAsync calls.
    private static async Task BufferRequestBodyAsync(HttpRequestMessage request)
    {
        if (request.Content == null) return;
        var bytes = await request.Content.ReadAsByteArrayAsync();
        var contentType = request.Content.Headers.ContentType;
        request.Content = new ByteArrayContent(bytes);
        if (contentType != null)
            request.Content.Headers.ContentType = contentType;
    }

    // Reads the current token from storage (or uses the supplied override) and sets the Authorization header.
    private async Task<TokenModel?> AttachBearerTokenAsync(
        HttpRequestMessage request,
        TokenModel? tokensOverride = null)
    {
        TokenModel? tokens;

        if (tokensOverride != null)
        {
            tokens = tokensOverride;
        }
        else
        {
            var localStorage = _circuitServicesAccesor?.Service?.GetRequiredService<ProtectedLocalStorage>();
            if (localStorage == null) return null;
            tokens = await localStorage.GetTokenAsync();
        }

        if (tokens is null) return null;

        if (!string.IsNullOrWhiteSpace(tokens.AccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        return tokens;
    }

    private async Task<TokenModel?> RefreshTokenAsync(TokenModel expiredTokens, CancellationToken cancellationToken)
    {
        var authService = _circuitServicesAccesor.Service?.GetRequiredService<AuthService>();
        if (authService == null)
        {
            _logger.LogError("AuthService not available for token refresh.");
            return null;
        }

        var refreshService = _circuitServicesAccesor.Service?.GetService<TokenRefreshService>();
        if (refreshService == null)
            return await authService.RefreshToken(expiredTokens, cancellationToken);

        var localStorage = _circuitServicesAccesor.Service?.GetRequiredService<ProtectedLocalStorage>();
        return await refreshService.TryRefreshAsync(
            expiredTokens,
            t => authService.RefreshToken(t, cancellationToken),
            async () => localStorage != null ? await localStorage.GetTokenAsync() : null,
            cancellationToken);
    }

    private async Task LogOutAsync()
    {
        if (_authStateProvider != null)
            await _authStateProvider.MarkUserAsLoggedOut();
    }
}
