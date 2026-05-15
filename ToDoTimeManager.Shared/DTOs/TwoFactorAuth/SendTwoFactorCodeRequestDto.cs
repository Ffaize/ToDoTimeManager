using ToDoTimeManager.Shared.Utils;

namespace ToDoTimeManager.Shared.DTOs.TwoFactorAuth;

public class SendTwoFactorCodeRequestDto
{
    [NotEmptyGuid]
    public Guid UserId { get; set; }
}
