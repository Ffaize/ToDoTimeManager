using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebUI.Services.Services.Implementations;

/// <summary>
/// Serialises concurrent token-refresh calls so that exactly one refresh request
/// is sent to the API even when multiple Blazor components or message handlers
/// detect an expired access token at the same time.
/// Registered as Scoped — one instance per Blazor circuit (= one per browser tab).
/// </summary>
public class TokenRefreshService(ILogger<TokenRefreshService> logger)
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Acquires the per-circuit lock and, if the access token has not been
    /// refreshed by a concurrent caller in the meantime, calls <paramref name="doRefresh"/>.
    /// </summary>
    /// <param name="tokensThatExpired">The token set observed before the lock was acquired.</param>
    /// <param name="doRefresh">The actual refresh API call.</param>
    /// <param name="readCurrentFromStorage">
    ///     Reads the latest token from storage — used for the double-check after acquiring the lock.
    /// </param>
    /// <param name="cancellationToken">Propagated to the semaphore wait and the refresh call.</param>
    public async Task<TokenModel?> TryRefreshAsync(
        TokenModel tokensThatExpired,
        Func<TokenModel, Task<TokenModel?>> doRefresh,
        Func<Task<TokenModel?>> readCurrentFromStorage,
        CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check: a concurrent caller may have already succeeded while we waited.
            var stored = await readCurrentFromStorage();
            if (stored?.AccessToken != null &&
                stored.AccessToken != tokensThatExpired.AccessToken)
            {
                logger.LogDebug("Access token was already refreshed by a concurrent call — reusing cached result.");
                return stored;
            }

            logger.LogDebug("Sending token refresh request to API.");
            return await doRefresh(tokensThatExpired);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Token refresh failed inside TokenRefreshService.");
            return null;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
