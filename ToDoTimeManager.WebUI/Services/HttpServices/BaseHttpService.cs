namespace ToDoTimeManager.WebUI.Services.HttpServices
{
    public abstract class BaseHttpService
    {
        protected readonly HttpClient _httpClient;
        protected string? ApiControllerName { get; set; }

        protected BaseHttpService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("TodoTimeManager");
        }

        protected string Url() => $"/api/{ApiControllerName}";

        protected string Url(string action) => $"/api/{ApiControllerName}/{action}";
    }
}
