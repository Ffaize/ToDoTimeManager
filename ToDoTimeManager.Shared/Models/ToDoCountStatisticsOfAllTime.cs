using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Shared.Models
{
    public class ToDoCountStatisticsOfAllTime
    {
        public ToDoStatus ToDoStatus { get; set; }
        public int Count { get; set; }
    }
}
