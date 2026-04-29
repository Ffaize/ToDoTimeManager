using ToDoTimeManager.Shared.Utils;

namespace ToDoTimeManager.Shared.DTOs;

public class SendTwoFactorCodeRequestDto
{
    [NotEmptyGuid]
    public Guid UserId { get; set; }
}
