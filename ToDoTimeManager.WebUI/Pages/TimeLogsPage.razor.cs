using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Components.Modals;
using ToDoTimeManager.WebUI.Localization;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;

namespace ToDoTimeManager.WebUI.Pages;

public partial class TimeLogsPage
{
    [Inject] private ToDosService ToDosService { get; set; } = null!;
    [Inject] private TimeLogsService TimeLogsService { get; set; } = null!;
    [Inject] private ToastsService ToastsService { get; set; } = null!;
    [Inject] private ILogger<TasksPage> Logger { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    private CustomAuthStateProvider AuthStateProvider => (CustomAuthStateProvider)AuthenticationStateProvider;

    public bool IsLogTimeEditModalVisible { get; set; }
    public bool IsDeleteTimeLogModalVisible { get; set; }
    public TimeFilter Filter
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            FilterData();
        }
    } = TimeFilter.AllTime;
    public string FilterText
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            FilterData();
        }
    } = string.Empty;

    public bool IsLogTimeModalVisible { get; set; }
    private List<TimeLog> AllLogs { get; set; } = [];
    private List<TimeLog> FilteredLogs { get; set; } = [];
    private List<ToDo> AllToDos { get; set; } = [];
    private TimeLog? EditableTimeLog { get; set; } = null;

    private ToDo? TaskCurrent =>
        AllToDos.FirstOrDefault(x => EditableTimeLog != null && x.Id == EditableTimeLog.ToDoId);

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
            FilterData();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void FilterData()
    {
        FilteredLogs = [.. AllLogs];
        if (!string.IsNullOrWhiteSpace(FilterText))
            FilteredLogs = AllLogs.Where(timeLog =>
                (timeLog.LogDescription != null && (timeLog.LogDescription.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                                                    timeLog.HoursSpent.ToString().Contains(FilterText, StringComparison.OrdinalIgnoreCase))) ||
                                                GetTaskNumber(timeLog).Contains(FilterText, StringComparison.OrdinalIgnoreCase)).ToList();

        if (Filter == TimeFilter.AllTime)
            return;
        var dateLimit = Filter switch
        {
            TimeFilter.DayAgo => DateTime.UtcNow.AddDays(-1),
            TimeFilter.WeekAgo => DateTime.UtcNow.AddDays(-7),
            TimeFilter.MonthAgo => DateTime.UtcNow.AddMonths(-1),
            TimeFilter.YearAgo => DateTime.UtcNow.AddYears(-1),
            _ => DateTime.MinValue
        };
        FilteredLogs = FilteredLogs.Where(toDo => toDo.LogDate >= dateLimit).ToList();
        InvokeAsync(StateHasChanged);
    }

    private async Task FetchData()
    {
        ShowLoader();
        var userCredentials = await AuthStateProvider.GetUserIdAndRoleAsync();
        if (userCredentials is null)
            return;
        AllToDos = await ToDosService.GetToDosByUserId(userCredentials.Value.Item1);
        AllLogs = await TimeLogsService.GetTimeLogsByUserId(userCredentials.Value.Item1);
        HideLoader();
        await InvokeAsync(StateHasChanged);
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

    private void OnLogTimeCreate(ModalResult obj)
    {
        IsLogTimeModalVisible = obj.Show;
        if (obj.Value is not null)
        {
            var timeLog = (TimeLog)obj.Value;
            var task = AllToDos.FirstOrDefault(x => x.NumberedId == (int)(obj.AdditionalValue ?? 0));
            if (task is null)
            {
                _ = ToastsService.ShowToast(Localizer["TaskWithThatNumberWereNotFound"].Value, true);
                InvokeAsync(StateHasChanged);
                return;
            }

            timeLog.ToDoId = task.Id;
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
        var createdTimeLog = await TimeLogsService.CreateTimeLog(timeLog);
        if (!createdTimeLog)
        {
            HideLoader();
            await InvokeAsync(StateHasChanged);
            return;
        }
        await ToastsService.ShowToast(Localizer["TimeLogCreated"], false);
        await FetchData();
        FilterData();
        HideLoader();
        await InvokeAsync(StateHasChanged);
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
            if (TaskCurrent != null) timeLog.ToDoId = TaskCurrent.Id;
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

    private string GetTaskNumber(TimeLog timeLog)
    {
        var task = AllToDos.FirstOrDefault(x => x.Id == timeLog.ToDoId);
        return task != null ? $"#{task.NumberedId}" : Localizer["TaskNotFound"].Value;
    }
}