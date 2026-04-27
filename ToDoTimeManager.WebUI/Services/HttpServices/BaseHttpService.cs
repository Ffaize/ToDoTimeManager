namespace ToDoTimeManager.WebUI.Services.HttpServices;

public abstract class BaseHttpService
{
    protected readonly HttpClient _httpClient;
    protected string? ApiControllerName { get; set; }

    protected BaseHttpService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("TodoTimeManager");
    }


    protected string Url()
    {
        return $"/api/{ApiControllerName}";
    }

    protected string Url(string action)
    {
        return $"/api/{ApiControllerName}/{action}";
    }
}