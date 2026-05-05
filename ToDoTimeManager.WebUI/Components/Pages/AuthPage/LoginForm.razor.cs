using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Models.Enums;

namespace ToDoTimeManager.WebUI.Components.Pages.AuthPage;

public partial class LoginForm
{
    [Parameter] public Action<AuthPageCurrentState>? GoTo { get; set; }
}