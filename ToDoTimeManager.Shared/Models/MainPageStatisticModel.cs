using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoTimeManager.Shared.Models
{
    public class MainPageStatisticModel
    {
        public List<TimeLog> TimeLogsForGivenTime { get; set; } = [];
        public List<TimeLog> TimeLogsForThisMonth { get; set; } = [];
        public Dictionary<DateTime, ToDo> DueDateTasks { get; set; } = new();
        public List<ToDoCountStatisticsOfAllTime> ToDoStatuses { get; set; } = [];
    }
}
