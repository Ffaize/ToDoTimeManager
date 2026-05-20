using Microsoft.AspNetCore.Components;
using ToDoTimeManager.Shared.DTOs.UserSettings;
using ToDoTimeManager.WebUI.Services.HttpServices;

namespace ToDoTimeManager.WebUI.Components.PageComponents.MainPage;

public partial class TotpSetupCard
{
    [Inject] private UserSettingsService UserSettingsService { get; set; } = null!;

    private TotpSetupResponseDto? _setup;
    private string[]              _confirmValues = new string[6];
    private bool                  _confirmed;

    private async Task OnGenerateClicked()
    {
        await Loading(async () =>
        {
            _setup = await UserSettingsService.GenerateTotpSetup();
        });
    }

    private async Task OnConfirmClicked()
    {
        if (_setup is null || _confirmValues.Any(string.IsNullOrEmpty)) return;

        await Loading(async () =>
        {
            var code    = string.Concat(_confirmValues);
            var success = await UserSettingsService.ConfirmTotpSetup(new ConfirmTotpSetupRequestDto
            {
                Secret = _setup.Secret,
                Code   = code
            });

            if (success)
            {
                _confirmed = true;
                _setup     = null;
            }
        });
    }
}
