using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Shared.DTOs
{
    public class MainPageStatisticRequestDto
    {
        public TimeFilter TimeFilter { get; set; }
        public Guid UserId { get; set; }
    }
}
