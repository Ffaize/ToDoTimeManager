using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Models.Enums;

namespace ToDoTimeManager.WebUI.Components.Pages.AuthPage;

public partial class TwoFaForm
{
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

    private string GetIsFilledCssClass(string value)
    {
        return string.IsNullOrEmpty(value) ? string.Empty : "filled";
    }
}