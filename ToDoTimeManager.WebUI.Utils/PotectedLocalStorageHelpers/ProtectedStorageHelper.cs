using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ToDoTimeManager.Shared.DTOs.User;
using ToDoTimeManager.WebUI.Models.Models;

namespace ToDoTimeManager.WebUI.Utils.PotectedLocalStorageHelpers;

public static class ProtectedStorageHelper
{
    public static async Task SaveLastLoginParameterAsync(this ProtectedLocalStorage storage, string loginParameter)
        => await storage.SetAsync("LastLoginParameter", loginParameter);

    public static async Task<string?> GetLastLoginParameterAsync(this ProtectedLocalStorage storage)
    {
        var result = await storage.GetAsync<string>("LastLoginParameter");
        return result is { Success: true, Value: not null } ? result.Value : null;
    }

    public static async Task SaveUserInfoAsync(this ProtectedLocalStorage storage, UserResponseDto user)
        => await storage.SetAsync("PendingTwoFaUser", user);

    public static async Task<UserResponseDto?> GetUserInfoAsync(this ProtectedLocalStorage storage)
    {
        var result = await storage.GetAsync<UserResponseDto>("PendingTwoFaUser");
        return result is { Success: true, Value: not null } ? result.Value : null;
    }

    public static async Task SavePendingTwoFaSessionStateAsync(this ProtectedLocalStorage storage, PendingTwoFaSessionState state)
        => await storage.SetAsync("PendingTwoFaSessionState", state);

    public static async Task<PendingTwoFaSessionState?> GetPendingTwoFaSessionStateAsync(this ProtectedLocalStorage storage)
    {
        var result = await storage.GetAsync<PendingTwoFaSessionState>("PendingTwoFaSessionState");
        return result is { Success: true, Value: not null } ? result.Value : null;
    }

    public static async Task RemovePendingTwoFaContextAsync(this ProtectedLocalStorage storage)
    {
        await storage.DeleteAsync("PendingTwoFaUser");
        await storage.DeleteAsync("PendingTwoFaSessionState");
    }
}
