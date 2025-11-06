namespace ToDoTimeManager.WebUI.Services.CircuitServicesAccesor;

public class CircuitServicesAccesor
{
    private static readonly AsyncLocal<IServiceProvider?> _blazoredService = new();

    public IServiceProvider? Service
    {
        get => _blazoredService.Value;
        set => _blazoredService.Value = value;
    }
}