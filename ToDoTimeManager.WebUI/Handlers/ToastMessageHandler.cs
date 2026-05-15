using ToDoTimeManager.WebUI.Services.Helpers.CircuitServicesAccesor;
using System.Text;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.Services.Interfaces;
using ToDoTimeManager.WebUI.Utils.OtherUtils;

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
            var toastsService = circuitServicesAccesor.Service.GetRequiredService<IToastsService>();

            var toastMessage = !string.IsNullOrWhiteSpace(content) && ProblemDetailsParser.TryExtract(content, out var extracted)
                ? extracted!
                : "Something went wrong";

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

}