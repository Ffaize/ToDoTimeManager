using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Components.Modals;
using ToDoTimeManager.WebUI.Localization;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;

namespace ToDoTimeManager.WebUI.Pages;

public partial class TaskDetailsPage
{
    [Inject] private ILogger<TaskDetailsPage> Logger { get; set; } = null!;
    [Inject] private ToDosService ToDosService { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;
    [Inject] private TimeLogsService TimeLogsService { get; set; } = null!;
    [Inject] private ToastsService ToastsService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    private CustomAuthStateProvider AuthStateProvider => (CustomAuthStateProvider)AuthenticationStateProvider;

    [Parameter] public Guid TaskId { get; set; }

    private ToDo Task { get; set; } = new();
    public bool IsTaskEditModalVisible { get; set; }
    public bool IsLogTimeEditModalVisible { get; set; }
    public List<TimeLog> TimeSpent { get; set; } = [];
    public TimeLog? EditableTimeLog { get; set; } = null;
    public bool IsLogTimeModalVisible { get; set; }
    public bool IsDeleteTimeLogModalVisible { get; set; }




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

    private void ShowModal(string nameOfBoolProp, object? additional = null)
    {
        try
        {

            if (additional is not null)
            {
                EditableTimeLog = (TimeLog)additional;
            }
            InvokeAsync(StateHasChanged);

            var prop = GetType().GetProperty(nameOfBoolProp);
            prop?.SetValue(this, true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
        }
        finally
        {
            InvokeAsync(StateHasChanged);
        }
    }

    private async Task FetchData()
    {
        try
        {
            ShowLoader();
            var task = await ToDosService.GetToDoById(TaskId);
            if (task is not null)
            {
                Task = task;
                var userIdAndRoleAsync = await AuthStateProvider.GetUserIdAndRoleAsync();
                if (userIdAndRoleAsync != null)
                    TimeSpent = await TimeLogsService.GetTimeLogsByUserIdAndToDoId(userIdAndRoleAsync.Value.Item1,
                        TaskId);
            }

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
        var userIdAndRoleAsync = await AuthStateProvider.GetUserIdAndRoleAsync();
        if (userIdAndRoleAsync != null) timeLog.UserId = userIdAndRoleAsync.Value.Item1;
        timeLog.ToDoId = TaskId;

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

    private string GetTimeSpent()
    {
        var totalTime = TimeSpent.Aggregate(TimeSpan.Zero, (current, log) => current + log.HoursSpent);
        return $"{(int)totalTime.TotalHours}h {totalTime.Minutes}m";
    }

    private void OnLogTimeEdit(ModalResult obj)
    {
        IsLogTimeEditModalVisible = obj.Show;
        if (obj.Value is not null)
        {
            var timeLog = (TimeLog)obj.Value;
            _ = UpdateTimeLog(timeLog);
        }
        StateHasChanged();
    }

    private async Task UpdateTimeLog(TimeLog timeLog)
    {
        ShowLoader();
        if (EditableTimeLog != null)
        {
            timeLog.Id = EditableTimeLog.Id;
            timeLog.LogDate = EditableTimeLog.LogDate;
            var description = timeLog.LogDescription;
            var userIdAndRoleAsync = await AuthStateProvider.GetUserIdAndRoleAsync();
            if (userIdAndRoleAsync != null) timeLog.UserId = userIdAndRoleAsync.Value.Item1;
            timeLog.ToDoId = TaskId;
            if (timeLog.LogDescription != null && !timeLog.LogDescription.Equals(description)) timeLog.LogDescription = description;
            var updateTimeLog = await TimeLogsService.UpdateTimeLog(timeLog);
            EditableTimeLog = null;
            if (!updateTimeLog)
            {
                HideLoader();
                await InvokeAsync(StateHasChanged);
                return;
            }
        }

        await ToastsService.ShowToast(Localizer["TimeLogUpdated"], false);

        await FetchData();
        HideLoader();
        await InvokeAsync(StateHasChanged);
    }

    private void OnDeleteProcesed(ModalResult obj)
    {
        IsDeleteTimeLogModalVisible = obj.Show;
        if (obj.Value is not null)
        {
            var res = (bool)obj.Value;
            if (res)
                _ = DeleteTimeLog();
        }
        StateHasChanged();
    }

    private async Task DeleteTimeLog()
    {
        ShowLoader();
        if (EditableTimeLog != null)
        {
            var deleteTimeLog = await TimeLogsService.DeleteTimeLog(EditableTimeLog.Id);
            EditableTimeLog = null;
            if (!deleteTimeLog)
            {
                HideLoader();
                await InvokeAsync(StateHasChanged);
                return;
            }
        }
        await ToastsService.ShowToast(Localizer["TimeLogDeleted"], false);
        await FetchData();
        HideLoader();
        await InvokeAsync(StateHasChanged);
    }
}