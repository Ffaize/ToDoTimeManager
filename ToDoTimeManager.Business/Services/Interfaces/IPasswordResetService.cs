using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Interfaces;

public interface IPasswordResetService
{
    Task<PasswordResetPendingModel> SendResetCode(string email);
    Task ResetPassword(Guid userId, string code, string newPassword);
}
