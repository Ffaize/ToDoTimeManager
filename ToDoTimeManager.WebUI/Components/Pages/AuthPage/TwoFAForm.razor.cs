using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ToDoTimeManager.WebUI.Models.Enums;

namespace ToDoTimeManager.WebUI.Components.Pages.AuthPage;

public partial class TwoFaForm
{
    [Inject] private IJSRuntime JS { get; set; } = null!;
    [Parameter] public Action<AuthPageCurrentState>? GoTo { get; set; }
    [Parameter] public string Email { get; set; }
    public string Value1
    {
        get;
        set
        {
            var upper = value.ToUpper();
            value = upper.Length > 1 ? upper.Substring(0, 1) : upper;
        }
    } = string.Empty;
    public string Value2
    {
        get;
        set
        {
            var upper = value.ToUpper();
            value = upper.Length > 1 ? upper.Substring(0, 1) : upper;
        }
    } = string.Empty;
    public string Value3
    {
        get;
        set
        {
            var upper = value.ToUpper();
            value = upper.Length > 1 ? upper.Substring(0, 1) : upper;
        }
    } = string.Empty;
    public string Value4
    {
        get;
        set
        {
            var upper = value.ToUpper();
            value = upper.Length > 1 ? upper.Substring(0, 1) : upper;
        }
    } = string.Empty;
    public string Value5
    {
        get;
        set
        {
            var upper = value.ToUpper();
            value = upper.Length > 1 ? upper.Substring(0, 1) : upper;
        }
    } = string.Empty;
    public string Value6
    {
        get;
        set
        {
            var upper = value.ToUpper();
            value = upper.Length > 1 ? upper.Substring(0, 1) : upper;
        }
    } = string.Empty;

    private void HandleInput(ChangeEventArgs e, int index)
    {
        var raw = e.Value?.ToString() ?? string.Empty;
        var val = raw.Length > 1 ? raw[..1].ToUpper() : raw.ToUpper();
        switch (index)
        {
            case 1: Value1 = val; break;
            case 2: Value2 = val; break;
            case 3: Value3 = val; break;
            case 4: Value4 = val; break;
            case 5: Value5 = val; break;
            case 6: Value6 = val; break;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JS.InvokeVoidAsync("initializeOtpInputs", "otp-inputs");
    }

    private string GetIsFilledCssClass(string value)
    {
        return string.IsNullOrEmpty(value) ? string.Empty : "filled";
    }
}