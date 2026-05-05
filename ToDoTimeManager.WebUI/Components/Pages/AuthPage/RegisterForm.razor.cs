using Microsoft.AspNetCore.Components;
using ToDoTimeManager.WebUI.Models.Enums;

namespace ToDoTimeManager.WebUI.Components.Pages.AuthPage;

public partial class RegisterForm
{
    [Parameter] public Action<AuthPageCurrentState>? GoTo { get; set; }
}