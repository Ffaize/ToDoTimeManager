using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.Shared.DTOs.UserSettings;
using ToDoTimeManager.Shared.Extensions;

namespace ToDoTimeManager.WebApi.Controllers;

[Authorize]
public class UserSettingsController : BaseController
{
    private readonly IUserSettingsService _userSettingsService;

    public UserSettingsController(IUserSettingsService userSettingsService)
    {
        _userSettingsService = userSettingsService;
    }

    [HttpGet("GetByUserId/{userId}")]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        var settings = await _userSettingsService.GetUserSettings(userId, GetCurrentUserId(), GetCurrentUserRole());
        return settings != null ? Ok(settings.ToResponseDto()) : StatusCode(500);
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] UpdateUserSettingsRequestDto request)
    {
        var result = await _userSettingsService.UpdateUserSettings(GetCurrentUserId(), request);
        return result ? Ok(result) : StatusCode(500);
    }

    [HttpGet("GetTwoFactorEnabled/{userId}")]
    public async Task<IActionResult> GetTwoFactorEnabled(Guid userId)
    {
        var result = await _userSettingsService.GetTwoFactorEnabled(userId, GetCurrentUserId(), GetCurrentUserRole());
        return result.HasValue ? Ok(result.Value) : StatusCode(500);
    }

    [HttpPut("SetTwoFactorEnabled")]
    public async Task<IActionResult> SetTwoFactorEnabled([FromBody] SetTwoFactorEnabledRequestDto request)
    {
        var result = await _userSettingsService.SetTwoFactorEnabled(GetCurrentUserId(), request.IsEnabled);
        return result ? Ok(result) : StatusCode(500);
    }

    [HttpGet("GenerateTotpSetup")]
    public async Task<IActionResult> GenerateTotpSetup()
    {
        var result = await _userSettingsService.GenerateTotpSetup(GetCurrentUserId());
        return Ok(result);
    }

    [HttpPost("ConfirmTotpSetup")]
    public async Task<IActionResult> ConfirmTotpSetup([FromBody] ConfirmTotpSetupRequestDto request)
    {
        await _userSettingsService.ConfirmTotpSetup(GetCurrentUserId(), request.Secret, request.Code);
        return Ok();
    }
}
