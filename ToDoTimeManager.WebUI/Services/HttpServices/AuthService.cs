using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebUI.Services.HttpServices
{
    public class AuthService : BaseHttpService
    {
        protected override string _apiControllerName { get; set; } = "auth";

        public AuthService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<TokenModel> RefreshToken(TokenModel tokens)
        {
            throw new NotImplementedException();
        }

    }
}
