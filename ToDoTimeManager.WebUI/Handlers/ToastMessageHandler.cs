using System;
using ToDoTimeManager.WebUI.Services.CircuitServicesAccesor;
using ToDoTimeManager.WebUI.Services.Implementations;

namespace ToDoTimeManager.WebUI.Handlers;

public class ToastMessageHandler(CircuitServicesAccesor circuitServicesAccesor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        try
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var isJson = IsJsonResponse(content);
            if (string.IsNullOrWhiteSpace(content) || isJson || circuitServicesAccesor.Service == null)
                return response;

            var toastMessagesService = circuitServicesAccesor.Service.GetRequiredService<ToastsService>();
            content = content.Replace("\"", string.Empty);
            await toastMessagesService.ShowToast(content, !response.IsSuccessStatusCode);

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return response;
        }
    }

    private static bool IsJsonResponse(string response)
    {
        return response.TrimStart().StartsWith("{") || response.TrimStart().StartsWith("[") || response.Contains("true")|| response.Contains("false");
    }
}