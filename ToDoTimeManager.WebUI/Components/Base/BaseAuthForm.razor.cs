using Microsoft.AspNetCore.Components;

namespace ToDoTimeManager.WebUI.Components.Base;

public partial class BaseAuthForm : BaseComponent
{
    [Parameter] public RenderFragment FormContent { get; set; }
    [Parameter] public List<string> Steps { get; set; }
    [Parameter] public string CurrentStep { get; set; }
    [Parameter] public RenderFragment MainIcon { get; set; }
    [Parameter] public RenderFragment MainText { get; set; }
    [Parameter] public RenderFragment SubText { get; set; }
    [Parameter] public string AditionalCssClass { get; set; }
}