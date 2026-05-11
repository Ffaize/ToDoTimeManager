using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ToDoTimeManager.WebUI.Models;

namespace ToDoTimeManager.WebUI.Utils;

public static class ProtectedStorageHelper
{
    public static async Task SaveLastLoginParameterAsync(this ProtectedLocalStorage storage, string loginParameter)
        => await storage.SetAsync("LastLoginParameter", loginParameter);

    public static async Task<string?> GetLastLoginParameterAsync(this ProtectedLocalStorage storage)
    {
        var result = await storage.GetAsync<string>("LastLoginParameter");
        return result is { Success: true, Value: not null } ? result.Value : null;
    }

    public static async Task SaveAuthPageStateAsync(this ProtectedLocalStorage storage, AuthPageSessionState state)
        => await storage.SetAsync("AuthPageState", state);

    public static async Task<AuthPageSessionState?> GetAuthPageStateAsync(this ProtectedLocalStorage storage)
    {
        var result = await storage.GetAsync<AuthPageSessionState>("AuthPageState");
        return result is { Success: true, Value: not null } ? result.Value : null;
    }

    public static async Task RemoveAuthPageStateAsync(this ProtectedLocalStorage storage)
        => await storage.DeleteAsync("AuthPageState");
}
