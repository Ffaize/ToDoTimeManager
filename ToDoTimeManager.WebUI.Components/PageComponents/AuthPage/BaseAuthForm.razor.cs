using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Components.BaseComponents;

namespace ToDoTimeManager.WebUI.Components.PageComponents.AuthPage;

public partial class BaseAuthForm : BaseComponent
{
    [Parameter] public RenderFragment? FormContent { get; set; }
    [Parameter] public string? AdditionalCssClass { get; set; }
}