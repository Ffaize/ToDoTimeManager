using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.WebUI.Components;
using ToDoTimeManager.WebUI.Localization;
using ToDoTimeManager.WebUI.Services.Implementations;

namespace ToDoTimeManager.WebUI.Pages;

public partial class MainPage
{
    [Inject] public ToastsService ToastService { get; set; } = null!;


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


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}