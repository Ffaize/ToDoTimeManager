using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using System;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebUI.Localization;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Implementations;

namespace ToDoTimeManager.WebUI.Pages;

public partial class MainPage
{
    [Inject] public ToastsService ToastService { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public StatisticService StatisticService { get; set; } = null!;
    [Inject] public ILogger<MainPage>  Logger{ get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    private CustomAuthStateProvider AuthStateProvider => (CustomAuthStateProvider)AuthenticationStateProvider;

    private int currentDay => DateTime.Now.Day; 


    #region BaseForComponent
    [Inject] private IStringLocalizer<Resource> Localizer { get; set; } = null!;
    public bool IsLoading { get; set; }

    public TimeFilter TimeFilter
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            _ = FetchData();
        }
    } = TimeFilter.WeekAgo;
    public MainPageStatisticModel MainPageStatistic { get; set; } = new();

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

    private async Task FetchData()
    {
        try
        {
            ShowLoader();
            var userCredentials = await AuthStateProvider.GetUserIdAndRoleAsync();
            var filter = new MainPageStatisticRequest()
            {
                TimeFilter = TimeFilter,
                UserId = userCredentials?.Item1 ?? Guid.Empty
            };
            MainPageStatistic = await StatisticService.GetMainPageStatistic(filter);
        }
        catch (Exception ex)
        {
            await ToastService.ShowToast(Localizer["ErrorWhileLoadingData"], true);
            Logger.LogError(ex, ex.Message);
        }
        finally
        {
            HideLoader();
        }
    }

    private string GetTimeSpent(TimeSpan timeSpent)
    {
        return $"{(int)timeSpent.TotalHours}h {timeSpent.Minutes}m";
    }

    private string GetTimeSpent()
    {
        if (MainPageStatistic.TimeLogsForGivenTime is {Count: < 1})
            return "0h 0m";
        var totalTime = MainPageStatistic.TimeLogsForGivenTime.Aggregate(TimeSpan.Zero, (current, log) => current + log.HoursSpent);
        return $"{(int)totalTime.TotalHours}h {totalTime.Minutes}m";
    }

    private string GetTimeLogBackground(int dayNumber)
    {
        if (MainPageStatistic.TimeLogsForThisMonth.Count == 0)
            return "bg-lightgray";
        var timeLogForDay = MainPageStatistic.TimeLogsForThisMonth
            .Where(x => x.LogDate.Day == dayNumber)
            .ToList();
        if (timeLogForDay.Count == 0)
            return "bg-lightgray";
        var totalTimeForDay = timeLogForDay.Aggregate(TimeSpan.Zero, (current, log) => current + log.HoursSpent);
        return totalTimeForDay.TotalHours switch
        {
            >= 8 => "bg-darkgreen",
            >= 4 => "bg-mediumgreen",
            > 0 => "bg-lightgreen",
            _ => "bg-lightgray"
        };
    }
}