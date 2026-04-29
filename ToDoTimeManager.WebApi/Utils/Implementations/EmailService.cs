using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using ToDoTimeManager.WebApi.Utils.Interfaces;

namespace ToDoTimeManager.WebApi.Utils.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendTwoFactorCodeAsync(string toEmail, string code)
    {
        var settings = _configuration.GetSection("EmailSettings");
        var host = settings["Host"]!;
        var port = int.Parse(settings["Port"]!);
        var senderEmail = settings["SenderEmail"]!;
        var senderName = settings["SenderName"]!;
        var password = settings["Password"]!;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Your verification code";
        message.Body = new TextPart("plain")
        {
            Text = $"Your verification code: {code}\n\nThis code expires in 5 minutes."
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(senderEmail, password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
