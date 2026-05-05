using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.WebUI.Components.Base;
using ToDoTimeManager.WebUI.Components.Modals;
using ToDoTimeManager.WebUI.Models.Enums;
using ToDoTimeManager.WebUI.Resources;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Interfaces;
using ToDoTimeManager.WebUI.Services.Modal;

namespace ToDoTimeManager.WebUI.Pages;

public partial class AuthPage
{
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private UserService UserService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private IToastsService ToastsService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILogger<AuthPage> Logger { get; set; } = null!;
    [Inject] private IModalService ModalService { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Loading(async () =>
            {
                await Task.Delay(10000);
            });
        }
        await base.OnAfterRenderAsync(firstRender);
    }


}