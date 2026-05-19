using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ToDoTimeManager.WebUI.Components.StaticComponents.Elements;

public partial class OtpInputRow
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter, EditorRequired] public string ContainerId { get; set; } = string.Empty;
    [Parameter] public string[] Values { get; set; } = new string[6];
    [Parameter] public EventCallback<string[]> ValuesChanged { get; set; }

    private async Task HandleInput(ChangeEventArgs e, int index)
    {
        var raw = e.Value?.ToString() ?? string.Empty;
        var val = raw.Length > 1 ? raw[..1].ToUpper() : raw.ToUpper();
        Values[index - 1] = val;
        await ValuesChanged.InvokeAsync(Values);
    }

    private string GetIsFilledCssClass(int index) =>
        string.IsNullOrEmpty(Values[index]) ? string.Empty : "filled";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JsRuntime.InvokeVoidAsync("initializeOtpInputs", ContainerId);
    }

    public async Task ReinitializeAsync()
    {
        await JsRuntime.InvokeVoidAsync("initializeOtpInputs", ContainerId);
    }
}
