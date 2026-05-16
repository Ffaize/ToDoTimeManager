using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Components.BaseComponents;

namespace ToDoTimeManager.WebUI.Components.PageComponents.AuthPage;

public partial class BaseAuthForm : BaseComponent
{
    [Parameter] public RenderFragment? FormContent { get; set; }
    [Parameter] public List<string>? Steps { get; set; }
    [Parameter] public string? CurrentStep { get; set; }
    [Parameter] public RenderFragment? MainIcon { get; set; }
    [Parameter] public RenderFragment? MainText { get; set; }
    [Parameter] public RenderFragment? SubText { get; set; }
    [Parameter] public string? AdditionalCssClass { get; set; }
}