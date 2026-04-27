using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.WebUI.Resources;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;

namespace ToDoTimeManager.WebUI.Pages;

public partial class MainPage
{
    [Inject] public ToastsService ToastService { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public StatisticService StatisticService { get; set; } = null!;
    [Inject] public ILogger<MainPage> Logger { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;


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