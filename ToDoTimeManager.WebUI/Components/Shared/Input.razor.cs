using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.WebUI.Models.Enums.Input;
using ToDoTimeManager.WebUI.Resources;
using ToDoTimeManager.WebUI.Utils;

namespace ToDoTimeManager.WebUI.Components.Shared;

public partial class Input
{
    [Inject] private IStringLocalizer<Resource> Localizer { get; set; } = null!;
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

    [Parameter] public bool UseValidation { get; set; }
    [Parameter] public Func<string?, string>? ValidationFunc { get; set; } = StringValidationHelper.DefaultValidation;
    [Parameter] public bool IsValid
    {
        get => HasValidationError; 
        set;
    }
    [Parameter] public EventCallback<bool> IsValidChanged { get; set; }


    private bool _showPassword;
    private ValidationState _validationState = ValidationState.None;
    private string _validationMessage = string.Empty;
    private bool HasValidationError => _validationState == ValidationState.Invalid && !string.IsNullOrEmpty(_validationMessage);

    private string GetInputType() => IsPassword
        ? (_showPassword ? "text" : "password")
        : Type;

    private void ToggleVisibility() => _showPassword = !_showPassword;

    private async Task HandleInput(ChangeEventArgs e)
    {
        await ValueChanged.InvokeAsync(e.Value?.ToString());
    }

    private void HandleFocusOut()
    {
        if (!UseValidation || ValidationFunc is null) return;

        _validationMessage = ValidationFunc(Value);
        _validationState = string.IsNullOrEmpty(_validationMessage)
            ? ValidationState.Valid
            : ValidationState.Invalid;
    }

    private string GetStyleClass() => Style switch
    {
        InputStyle.Ghost => "input-shell--ghost",
        _ => string.Empty
    };

    private string GetValidationClass() => _validationState switch
    {
        ValidationState.Valid => "input-shell--valid",
        ValidationState.Invalid => "input-shell--invalid",
        _ => string.Empty
    };

    private enum ValidationState { None, Valid, Invalid }
}