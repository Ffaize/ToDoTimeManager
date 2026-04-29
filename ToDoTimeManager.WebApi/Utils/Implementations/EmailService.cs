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

        var lifetimeMinutes = _configuration["TwoFactorSettings:CodeLifetimeMinutes"] ?? "5";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Your verification code — ToDoTimeManager";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $"""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                  <meta charset="UTF-8" />
                  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                  <title>Verification Code</title>
                </head>
                <body style="margin:0;padding:0;background-color:#f4f6f8;font-family:Arial,Helvetica,sans-serif;">
                  <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f4f6f8;padding:40px 0;">
                    <tr>
                      <td align="center">
                        <table width="520" cellpadding="0" cellspacing="0" style="background-color:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,0.08);">
                          <!-- Header -->
                          <tr>
                            <td style="background-color:#4f46e5;padding:32px 40px;text-align:center;">
                              <h1 style="margin:0;color:#ffffff;font-size:22px;font-weight:700;letter-spacing:0.5px;">
                                ToDoTimeManager
                              </h1>
                            </td>
                          </tr>
                          <!-- Body -->
                          <tr>
                            <td style="padding:40px 40px 24px;">
                              <p style="margin:0 0 8px;font-size:16px;color:#374151;font-weight:600;">
                                Two-Factor Verification
                              </p>
                              <p style="margin:0 0 24px;font-size:14px;color:#6b7280;line-height:1.6;">
                                Use the code below to complete your sign-in. It is valid for
                                <strong>{lifetimeMinutes} minutes</strong> and can only be used once.
                              </p>
                              <!-- Code box -->
                              <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                  <td align="center" style="background-color:#f0f0ff;border:2px dashed #4f46e5;border-radius:8px;padding:24px;">
                                    <span style="font-size:36px;font-weight:700;letter-spacing:8px;color:#4f46e5;font-family:'Courier New',monospace;">
                                      {code}
                                    </span>
                                  </td>
                                </tr>
                              </table>
                              <p style="margin:24px 0 0;font-size:13px;color:#9ca3af;line-height:1.5;">
                                If you did not request this code, you can safely ignore this email.
                                Someone may have entered your email by mistake.
                              </p>
                            </td>
                          </tr>
                          <!-- Footer -->
                          <tr>
                            <td style="padding:16px 40px 32px;text-align:center;border-top:1px solid #f3f4f6;">
                              <p style="margin:0;font-size:12px;color:#d1d5db;">
                                &copy; {DateTime.UtcNow.Year} ToDoTimeManager. All rights reserved.
                              </p>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
                </html>
                """
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var cancellationTokenSource = new System.Threading.CancellationTokenSource(System.TimeSpan.FromSeconds(30));
        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls, cancellationTokenSource.Token);
        await client.AuthenticateAsync(senderEmail, password, cancellationTokenSource.Token);
        await client.SendAsync(message, cancellationTokenSource.Token);
        await client.DisconnectAsync(true, cancellationTokenSource.Token);
    }
}
