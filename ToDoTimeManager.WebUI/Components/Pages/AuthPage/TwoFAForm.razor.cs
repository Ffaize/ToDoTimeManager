using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;

namespace ToDoTimeManager.WebUI.Components.Pages.AuthPage;

public partial class TwoFAForm
{
    [Inject] private IJSRuntime JS { get; set; } = null!;
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter] public Action<AuthPageCurrentState>? GoTo { get; set; }
    [Parameter] public string Email { get; set; } = string.Empty;
    [Parameter] public Guid UserId { get; set; }
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

    private async Task OnVerifyClicked()
    {
        if (string.IsNullOrEmpty(Value1) || string.IsNullOrEmpty(Value2) || string.IsNullOrEmpty(Value3) ||
            string.IsNullOrEmpty(Value4) || string.IsNullOrEmpty(Value5) || string.IsNullOrEmpty(Value6)) return;

        await Loading(async () =>
        {
            var code = $"{Value1}{Value2}{Value3}-{Value4}{Value5}{Value6}";
            var tokens = await AuthService.VerifyCode(new VerifyTwoFactorRequestDto
            {
                UserId = UserId,
                Code = code
            });

            if (tokens is null) return;

            var authProvider = (CustomAuthStateProvider)AuthenticationStateProvider;
            await authProvider.MarkUserAsAuthenticated(tokens);
            NavigationManager.NavigateTo("/dashboard");
        });
    }

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