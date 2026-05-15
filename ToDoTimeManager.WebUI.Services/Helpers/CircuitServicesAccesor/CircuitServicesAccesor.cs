namespace ToDoTimeManager.WebUI.Services.Helpers.CircuitServicesAccesor;

public class CircuitServicesAccesor
{
    private static readonly AsyncLocal<IServiceProvider?> _blazoredService = new();

    public IServiceProvider? Service
    {
        get => _blazoredService.Value;
        set => _blazoredService.Value = value;
    }
}