using ToDoTimeManager.Shared.DTOs.UserSettings;

namespace ToDoTimeManager.WebUI.Services.HttpServices;

public class UserSettingsService : BaseHttpService
{
    private readonly ILogger<UserSettingsService> _logger;

    public UserSettingsService(IHttpClientFactory httpClientFactory, ILogger<UserSettingsService> logger)
        : base(httpClientFactory)
    {
        _logger           = logger;
        ApiControllerName = "UserSettings";
    }

    public async Task<TotpSetupResponseDto?> GenerateTotpSetup()
    {
        try
        {
            var response = await _httpClient.GetAsync(Url("GenerateTotpSetup"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TotpSetupResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate TOTP setup");
            return null;
        }
    }

    public async Task<bool> ConfirmTotpSetup(ConfirmTotpSetupRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(Url("ConfirmTotpSetup"), request);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to confirm TOTP setup");
            return false;
        }
    }
}
