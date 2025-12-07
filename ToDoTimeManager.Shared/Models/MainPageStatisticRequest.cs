using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Shared.Models
{
    public class MainPageStatisticRequest
    {
        public TimeFilter TimeFilter { get; set; }
        public Guid UserId { get; set; }
    }
}
