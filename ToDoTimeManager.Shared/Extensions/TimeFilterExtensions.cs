using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Shared.Extensions;

public static class TimeFilterExtensions
{
    public static int ToDaysAgo(this TimeFilter filter) =>
        filter switch
        {
            TimeFilter.DayAgo => 1,
            TimeFilter.WeekAgo => 7,
            TimeFilter.MonthAgo => 30,
            TimeFilter.YearAgo => 365,
            _ => -1
        };
}
