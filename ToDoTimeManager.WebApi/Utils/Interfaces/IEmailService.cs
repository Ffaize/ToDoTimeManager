namespace ToDoTimeManager.WebApi.Utils.Interfaces;

public interface IEmailService
{
    Task SendTwoFactorCodeAsync(string toEmail, string code);
}
