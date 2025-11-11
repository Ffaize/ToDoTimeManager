using System.Net.Http.Headers;

namespace ToDoTimeManager.WebUI.Services.HttpServices
{
    public abstract class BaseHttpService
    {
        protected readonly HttpClient _httpClient;
        protected abstract string _apiControllerName { get; set; }

        protected BaseHttpService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("TodoTimeManager");
        }

        protected string Url() => $"/api/{_apiControllerName}";

        protected string Url(string action) => $"/api/{_apiControllerName}/{action}";
    }
}
