using ToDoTimeManager.WebUI.Services.CircuitServicesAccesor;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.Interfaces;

namespace ToDoTimeManager.WebUI.Handlers;

public class ToastMessageHandler(CircuitServicesAccesor circuitServicesAccesor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        // We only show toasts on non-success responses.
        if (response.IsSuccessStatusCode || circuitServicesAccesor.Service == null)
            return response;

        try
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            // Restore content for downstream readers (e.g. ReadFromJsonAsync in services).
            var mediaType = response.Content.Headers.ContentType?.MediaType;
            response.Content = new StringContent(content ?? string.Empty, Encoding.UTF8);
            if (!string.IsNullOrWhiteSpace(mediaType))
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

            if (string.IsNullOrWhiteSpace(content))
                return response;

            var message = ExtractMessageFromJsonOrPlainText(content);
            if (string.IsNullOrWhiteSpace(message))
                message = content.Replace("\"", string.Empty);

            var toastMessagesService = circuitServicesAccesor.Service.GetRequiredService<IToastsService>();
            await toastMessagesService.ShowToast(message, ToastType.Error);
            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return response;
        }
    }

    private static string? ExtractMessageFromJsonOrPlainText(string content)
    {
        // Fast path: plain text
        var trimmed = content.TrimStart();
        if (!(trimmed.StartsWith("{") || trimmed.StartsWith("[")))
            return content.Replace("\"", string.Empty).Trim();

        try
        {
            using var doc = JsonDocument.Parse(content);
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
                return content.Replace("\"", string.Empty).Trim();

            // ProblemDetails: { "title": "...", "detail": "...", "status": ... }
            if (doc.RootElement.TryGetProperty("detail", out var detailProp) &&
                detailProp.ValueKind == JsonValueKind.String)
                return detailProp.GetString();

            if (doc.RootElement.TryGetProperty("title", out var titleProp) &&
                titleProp.ValueKind == JsonValueKind.String)
                return titleProp.GetString();

            // ValidationProblemDetails: { "errors": { "Field": [ "msg1", ... ] } }
            if (doc.RootElement.TryGetProperty("errors", out var errorsProp) &&
                errorsProp.ValueKind == JsonValueKind.Object)
                foreach (var property in errorsProp.EnumerateObject())
                {
                    var arr = property.Value;
                    if (arr.ValueKind == JsonValueKind.Array && arr.GetArrayLength() > 0)
                    {
                        var first = arr[0];
                        if (first.ValueKind == JsonValueKind.String)
                            return first.GetString();
                    }
                }

            // Fallback: whole JSON as string
            return content.Replace("\"", string.Empty).Trim();
        }
        catch
        {
            return content.Replace("\"", string.Empty).Trim();
        }
    }
}