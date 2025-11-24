using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.Shared.Utils;
using ToDoTimeManager.WebUI.Localization;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Utils;

namespace ToDoTimeManager.WebUI.Pages;

public partial class TasksPage
{
    [Inject] private ToDosService ToDosService { get; set; } = null!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = null!;
    [Parameter] public ToDoFilter Filter { get; set; } = ToDoFilter.All;
    private List<ToDo> AllToDos { get; set; } = [];
    private List<ToDo> FilteredToDos { get; set; } = [];

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

            FilteredToDos = new List<ToDo>
{
    new ToDo { Id = Guid.NewGuid(), NumberedId = 1, Title = "Buy groceries", Description = "Milk, bread, eggs", CreatedAt = DateTime.UtcNow.AddDays(-3), DueDate = DateTime.UtcNow.AddDays(2), Status = ToDoStatus.InProgress, AssignedTo = Guid.NewGuid() },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 2, Title = "Fix laptop", Description = "Replace thermal paste", CreatedAt = DateTime.UtcNow.AddDays(-10), DueDate = DateTime.UtcNow.AddDays(1), Status = ToDoStatus.Completed, AssignedTo = null },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 3, Title = "Finish API module", Description = "Implement StatisticService", CreatedAt = DateTime.UtcNow.AddDays(-1), DueDate = null, Status = ToDoStatus.Completed, AssignedTo = Guid.NewGuid() },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 4, Title = "Workout", Description = "Leg day at gym", CreatedAt = DateTime.UtcNow.AddDays(-2), DueDate = DateTime.UtcNow.AddDays(7), Status = ToDoStatus.InProgress, AssignedTo = null },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 5, Title = "Clean room", Description = "Organize desk & shelves", CreatedAt = DateTime.UtcNow.AddDays(-4), DueDate = null, Status = ToDoStatus.InProgress, AssignedTo = Guid.NewGuid() },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 6, Title = "Send report", Description = "Weekly work summary", CreatedAt = DateTime.UtcNow.AddDays(-8), DueDate = DateTime.UtcNow.AddDays(-1), Status = ToDoStatus.Cancelled, AssignedTo = Guid.NewGuid() },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 7, Title = "Walk the dog", Description = "Evening walk", CreatedAt = DateTime.UtcNow.AddHours(-12), DueDate = null, Status = ToDoStatus.Completed, AssignedTo = null },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 8, Title = "Update Docker images", Description = "Rebuild API + UI", CreatedAt = DateTime.UtcNow.AddDays(-6), DueDate = DateTime.UtcNow.AddDays(3), Status = ToDoStatus.New, AssignedTo = Guid.NewGuid() },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 9, Title = "Study MAUI", Description = "SkiaSharp circular control", CreatedAt = DateTime.UtcNow.AddDays(-5), DueDate = null, Status = ToDoStatus.InProgress, AssignedTo = null },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 10, Title = "Pay bills", Description = "Electricity & internet", CreatedAt = DateTime.UtcNow.AddDays(-2), DueDate = DateTime.UtcNow.AddDays(5), Status = ToDoStatus.OnHold, AssignedTo = Guid.NewGuid() },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 11, Title = "Test refresh token", Description = "Implement automatic refresh", CreatedAt = DateTime.UtcNow, DueDate = null, Status = ToDoStatus.InProgress, AssignedTo = Guid.NewGuid() },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 12, Title = "Learn RabbitMQ", Description = "Queue and consumer basics", CreatedAt = DateTime.UtcNow.AddDays(-9), DueDate = null, Status = ToDoStatus.InProgress, AssignedTo = null },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 13, Title = "Fix CSS animation bug", Description = "Prevent restart on state change", CreatedAt = DateTime.UtcNow.AddDays(-3), DueDate = null, Status = ToDoStatus.Completed, AssignedTo = Guid.NewGuid() },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 14, Title = "Replace car battery", Description = "Buy new one", CreatedAt = DateTime.UtcNow.AddDays(-12), DueDate = DateTime.UtcNow.AddDays(4), Status = ToDoStatus.InProgress, AssignedTo = null },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 15, Title = "Read a book", Description = "Finish 50 pages", CreatedAt = DateTime.UtcNow.AddDays(-1), DueDate = null, Status = ToDoStatus.InProgress, AssignedTo = null },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 16, Title = "Optimize SQL queries", Description = "Review stored procedures", CreatedAt = DateTime.UtcNow.AddDays(-15), DueDate = DateTime.UtcNow.AddDays(-5), Status = ToDoStatus.New, AssignedTo = Guid.NewGuid() },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 17, Title = "Fix Blazor onclick error", Description = "Component parameter mismatch", CreatedAt = DateTime.UtcNow.AddDays(-7), DueDate = null, Status = ToDoStatus.Completed, AssignedTo = Guid.NewGuid() },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 18, Title = "Design new UI", Description = "Dark e-shop theme", CreatedAt = DateTime.UtcNow.AddDays(-11), DueDate = DateTime.UtcNow.AddDays(6), Status = ToDoStatus.InProgress, AssignedTo = Guid.NewGuid() },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 19, Title = "Create toast service", Description = "Reusable component", CreatedAt = DateTime.UtcNow.AddDays(-4), DueDate = null, Status = ToDoStatus.OnHold, AssignedTo = null },
    new ToDo { Id = Guid.NewGuid(), NumberedId = 20, Title = "Backup database", Description = "Full backup to external drive", CreatedAt = DateTime.UtcNow.AddDays(-13), DueDate = DateTime.UtcNow.AddDays(2), Status = ToDoStatus.InProgress, AssignedTo = Guid.NewGuid() }
};
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task FetchData()
    {
        ShowLoader();
        var accessToken = (await ProtectedLocalStorage.GetTokenAsync())?.AccessToken;
        if (accessToken is null)
            return;
        var (userId, role) =
            JwtTokenHelper.GetUserDataFromAccessToken(accessToken);
        if (userId is null)
            return;
        AllToDos = await ToDosService.GetToDosByUserId(Guid.Parse(userId));
        HideLoader();
        await InvokeAsync(StateHasChanged);
    }
}