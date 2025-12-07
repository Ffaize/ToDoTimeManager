using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.Shared.Utils;
using ToDoTimeManager.WebUI.Components.Modals;
using ToDoTimeManager.WebUI.Localization;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;
using ToDoTimeManager.WebUI.Utils;

namespace ToDoTimeManager.WebUI.Pages;

public partial class TasksPage
{
    [Inject] private ToDosService ToDosService { get; set; } = null!;
    [Inject] private TimeLogsService TimeLogsService { get; set; } = null!;
    [Inject] private ToastsService ToastsService { get; set; } = null!;
    [Inject] private ILogger<TasksPage> Logger { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    private CustomAuthStateProvider AuthStateProvider => (CustomAuthStateProvider)AuthenticationStateProvider;


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

    private List<ToDo> AllToDos { get; set; } = [];
    private List<ToDo> FilteredToDos { get; set; } = [];
    public bool IsTaskCreateModalVisible { get; set; }
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
            FilterData();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void FilterData()
    {
        FilteredToDos = [.. AllToDos];
        if (!string.IsNullOrWhiteSpace(FilterText))
            FilteredToDos = AllToDos.Where(toDo => Localizer[GetNameWithStatus(toDo)].Value!.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                                                   toDo.Title!.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                                                   toDo.NumberedId.ToString().Contains(FilterText, StringComparison.OrdinalIgnoreCase)).ToList();

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
        FilteredToDos = FilteredToDos.Where(toDo => toDo.CreatedAt >= dateLimit).ToList();
        InvokeAsync(StateHasChanged);
    }

    private static string GetNameWithStatus(ToDo toDo)
    {
        var status = Enum.GetName(toDo.Status) ?? string.Empty;
        return status += "Status";
    }

    private async Task FetchData()
    {
        ShowLoader();
        var userCredentials = await AuthStateProvider.GetUserIdAndRoleAsync();
        if (userCredentials is null)
            return;
        AllToDos = await ToDosService.GetToDosByUserId(userCredentials.Value.Item1);
        HideLoader();
        await InvokeAsync(StateHasChanged);
    }

    private void GoToTaskDetailPage(Guid toDoId)
    {
        NavigationManager.NavigateTo($"/taskDetails/{toDoId}");
    }

    private void OnTaskCreateChanged(ModalResult obj)
    {
        IsTaskCreateModalVisible = obj.Show;
        if (obj.Value is not null)
        {
            var newToDo = (ToDo)obj.Value;
            _ = CreateToDo(newToDo);
        }
        StateHasChanged();
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

    private async Task CreateToDo(ToDo newToDo)
    {
        ShowLoader();
        newToDo.Id = Guid.NewGuid();
        newToDo.CreatedAt = DateTime.UtcNow;
        var accessToken = (await ProtectedLocalStorage.GetTokenAsync())?.AccessToken;
        if (accessToken is null)
            return;
        var (userId, role) =
            JwtTokenHelper.GetUserDataFromAccessToken(accessToken);
        newToDo.AssignedTo = Guid.Parse((ReadOnlySpan<char>)userId);

        var createdToDo = await ToDosService.CreateToDo(newToDo);
        if (!createdToDo)
        {
            HideLoader();
            await InvokeAsync(StateHasChanged);
            return;
        }
        await ToastsService.ShowToast(Localizer["TaskCreated"], false);
        await FetchData();
        FilterData();
        HideLoader();
        GoToTaskDetailPage(newToDo.Id);
        await InvokeAsync(StateHasChanged);
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
                ToastsService.ShowToast(Localizer["TaskWithThatNumberWereNotFound"].Value, true).GetAwaiter().GetResult();
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
}