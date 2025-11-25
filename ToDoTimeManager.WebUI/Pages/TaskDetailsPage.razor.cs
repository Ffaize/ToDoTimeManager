using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Localization;
using ToDoTimeManager.WebUI.Services.HttpServices;

namespace ToDoTimeManager.WebUI.Pages;

public partial class TaskDetailsPage
{
    [Inject] private ILogger<TaskDetailsPage> Logger { get; set; } = null!;
    [Inject] private ToDosService ToDosService { get; set; } = null!;
    [Parameter] public Guid TaskId { get; set; }

    private ToDo Task { get; set; } = new();
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