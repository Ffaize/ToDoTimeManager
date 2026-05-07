using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Models.Enums.Input;

namespace ToDoTimeManager.WebUI.Components.Shared;

public partial class Input
{
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public string Type { get; set; } = "text";
    [Parameter] public bool IsPassword { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string? AdditionalCssClass { get; set; }
    [Parameter] public RenderFragment? Icon { get; set; }
    [Parameter] public InputIconPosition IconPosition { get; set; } = InputIconPosition.Left;
    [Parameter] public InputStyle Style { get; set; } = InputStyle.Default;
    private bool _showPassword;

    private string GetInputType() => IsPassword
        ? (_showPassword ? "text" : "password")
        : Type;

    private void ToggleVisibility() => _showPassword = !_showPassword;

    private async Task HandleInput(ChangeEventArgs e)
    {
        await ValueChanged.InvokeAsync(e.Value?.ToString());
    }

    private string GetStyleClass() => Style switch
    {
        InputStyle.Ghost => "input-shell--ghost",
        _ => string.Empty
    };
}