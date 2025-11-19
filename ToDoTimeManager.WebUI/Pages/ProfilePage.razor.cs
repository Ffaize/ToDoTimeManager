using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.WebUI.Localization;

namespace ToDoTimeManager.WebUI.Pages;

public partial class ProfilePage
{
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