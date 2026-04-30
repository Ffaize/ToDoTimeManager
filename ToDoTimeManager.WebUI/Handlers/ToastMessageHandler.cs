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

        if (response.IsSuccessStatusCode || circuitServicesAccesor.Service == null)
            return response;

        try
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(content))
                return response;

            var toastsService = circuitServicesAccesor.Service.GetRequiredService<IToastsService>();

            var toastMessage = TryExtractProblemDetails(content, out var extracted)
                ? extracted!
                : "Smth went wrong";

            await toastsService.ShowToast(toastMessage, ToastType.Error);
            response.Content = new StringContent("null", Encoding.UTF8, "application/json");
            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return response;
        }
    }

    private static bool TryExtractProblemDetails(string content, out string? message)
    {
        message = null;

        if (!content.TrimStart().StartsWith("{"))
            return false;

        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
                return false;

            // ProblemDetails must have a numeric "status" field.
            if (!root.TryGetProperty("status", out var statusProp) ||
                statusProp.ValueKind != JsonValueKind.Number)
                return false;

            if (root.TryGetProperty("detail", out var detailProp) &&
                detailProp.ValueKind == JsonValueKind.String)
            {
                message = detailProp.GetString();
                return true;
            }

            // ValidationProblemDetails: { "errors": { "Field": ["msg", ...] } }
            if (root.TryGetProperty("errors", out var errorsProp) &&
                errorsProp.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in errorsProp.EnumerateObject())
                {
                    var arr = prop.Value;
                    if (arr.ValueKind == JsonValueKind.Array && arr.GetArrayLength() > 0 &&
                        arr[0].ValueKind == JsonValueKind.String)
                    {
                        message = arr[0].GetString();
                        return true;
                    }
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}