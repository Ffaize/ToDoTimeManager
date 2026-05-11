using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

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
}
