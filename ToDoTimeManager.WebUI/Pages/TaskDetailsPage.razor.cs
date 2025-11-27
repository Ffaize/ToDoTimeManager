using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.Shared.Utils;
using ToDoTimeManager.WebUI.Components.Modals;
using ToDoTimeManager.WebUI.Localization;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;
using ToDoTimeManager.WebUI.Utils;

namespace ToDoTimeManager.WebUI.Pages;

public partial class TaskDetailsPage
{
    [Inject] private ILogger<TaskDetailsPage> Logger { get; set; } = null!;
    [Inject] private ToDosService ToDosService { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;
    [Inject] private TimeLogsService TimeLogsService { get; set; } = null!;
    [Inject] private ToastsService ToastsService { get; set; } = null!;

    [Parameter] public Guid TaskId { get; set; }

    private ToDo Task { get; set; } = new();
    public bool IsTaskEditModalVisible { get; set; }
    public bool IsLogTimeModalVisible { get; set; }


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
            await FetchData();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void OnTaskCreateChanged(ModalResult obj)
    {
        IsTaskEditModalVisible = obj.Show;
        if (obj.Value is not null)
        {
            var newToDo = (ToDo)obj.Value;
            _ = EditToDo(newToDo);
        }
        StateHasChanged();
    }

    private async Task EditToDo(ToDo newToDo)
    {
        try
        {
            ShowLoader();
            var updatedToDo = await ToDosService.UpdateToDo(newToDo);
            if (updatedToDo)
                await FetchData();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating task details for TaskId: {TaskId}", TaskId);
        }
        finally
        {
            HideLoader();
            await InvokeAsync(StateHasChanged);
        }
    }

    private void ShowModal(string nameOfBoolProp)
    {
        try
        {
            var prop = GetType().GetProperty(nameOfBoolProp);
            prop?.SetValue(this, true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
        }
    }

    private async Task FetchData()
    {
        try
        {
            ShowLoader();
            var task =  await ToDosService.GetToDoById(TaskId);
            if (task is not null)
                Task = task;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching task details for TaskId: {TaskId}", TaskId);
        }
        finally
        {
            HideLoader();
            await InvokeAsync(StateHasChanged);
        }
    }

    private void OnLogTimeCreate(ModalResult obj)
    {
        IsLogTimeModalVisible = obj.Show;
        if (obj.Value is not null)
        {
            var timeLog = (TimeLog)obj.Value;
            _ = CreateTimeLog(timeLog);
        }
        StateHasChanged();
    }

    private async Task CreateTimeLog(TimeLog timeLog)
    {
        ShowLoader();
        timeLog.Id = Guid.NewGuid();
        timeLog.LogDate = DateTime.UtcNow;
        var accessToken = (await ProtectedLocalStorage.GetTokenAsync())?.AccessToken;
        if (accessToken is null)
            return;
        var (userId, role) =
            JwtTokenHelper.GetUserDataFromAccessToken(accessToken);
        timeLog.UserId = Guid.Parse((ReadOnlySpan<char>)userId);
        var createdTimeLog = await TimeLogsService.CreateTimeLog(timeLog);
        if (!createdTimeLog)
        {
            HideLoader();
            await InvokeAsync(StateHasChanged);
            return;
        }
        await ToastsService.ShowToast(Localizer["TimeLogCreated"], false);
        await FetchData();
        HideLoader();
        await InvokeAsync(StateHasChanged);
    }
}