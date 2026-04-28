using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
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
            await Task.Delay(1000);
            await ToastsService.ShowToast(Localizer["AuthPageLoaded"], ToastType.Error);
            await Task.Delay(1000);
            await ToastsService.ShowToast(Localizer["AuthPageLoaded"], ToastType.Success);
            await Task.Delay(1000);
            await ToastsService.ShowToast(Localizer["AuthPageLoaded"], ToastType.Warning);
            await Task.Delay(5000);
            var parameters = new ModalParameters();
            parameters.Add("MessageDetails", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

            var modal = ModalService.Show<ConfirmModal>("Заголовок", parameters);
            var confirmed = await modal.Result;

        }
        await base.OnAfterRenderAsync(firstRender);
    }


    #region BaseForComponent

    [Inject] private IStringLocalizer<Resource> Localizer { get; set; } = null!;
    public bool IsLoading { get; set; }

    public void ShowLoader()
    {
        IsLoading = true;
        InvokeAsync(StateHasChanged);
    }

    public void HideLoader()
    {
        IsLoading = false;
        InvokeAsync(StateHasChanged);
    }

    #endregion
}