using System.Text.Json;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebUI.Utils
{
    public static class TokenProtectedStorageHelper
    {
        public static async Task SaveTokenAsync(this ProtectedLocalStorage protectedLocalStorage, TokenModel token)
        {
            var tokenJson = JsonSerializer.Serialize(token);
            await protectedLocalStorage.SetAsync(nameof(TokenModel), tokenJson);
        }

        public static async Task<TokenModel?> GetTokenAsync(this ProtectedLocalStorage protectedLocalStorage)
        {
            var result = await protectedLocalStorage.GetAsync<string>(nameof(TokenModel));

            return result is { Success: true, Value: not null } ? JsonSerializer.Deserialize<TokenModel>(result.Value) : null;
        }

        public static async Task RemoveTokenAsync(this ProtectedLocalStorage protectedLocalStorage)
        {
            await protectedLocalStorage.DeleteAsync(nameof(TokenModel));
        }
    }
}
