using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Shared.Models
{
    public class MainPageStatisticRequest
    {
        public TimeFilter TimeFilter { get; set; }
        public Guid UserId { get; set; }
    }
}
