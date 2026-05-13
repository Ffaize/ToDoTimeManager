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

        var d1 = code.Length > 0 ? code[0].ToString() : "";
        var d2 = code.Length > 1 ? code[1].ToString() : "";
        var d3 = code.Length > 2 ? code[2].ToString() : "";
        var d4 = code.Length > 4 ? code[4].ToString() : "";
        var d5 = code.Length > 5 ? code[5].ToString() : "";
        var d6 = code.Length > 6 ? code[6].ToString() : "";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $$"""
                <!DOCTYPE html>
                <html lang="en">
                
                <head>
                    <meta charset="UTF-8" />
                    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                    <title>Verification Code — TaskForge</title>
                    <link
                        href="https://fonts.googleapis.com/css2?family=Space+Grotesk:wght@500;600;700&family=JetBrains+Mono:wght@400;600&display=swap"
                        rel="stylesheet" />
                    <style>
                        /* Tablet */
                        @media only screen and (max-width: 680px) {
                            .outer-table {
                                width: 100% !important;
                            }
                
                            .card {
                                border-radius: 12px !important;
                            }
                
                            .panel {
                                padding: 32px 28px !important;
                            }
                        }
                
                        /* Large phone */
                        @media only screen and (max-width: 540px) {
                            .card {
                                border-radius: 0 !important;
                                border-left: none !important;
                                border-right: none !important;
                            }
                
                            .panel {
                                padding: 28px 22px !important;
                            }
                
                            .title {
                                font-size: 20px !important;
                            }
                
                            .otp-cell {
                                width: 42px !important;
                                height: 50px !important;
                                font-size: 20px !important;
                            }
                
                            .otp-dash {
                                width: 16px !important;
                                font-size: 15px !important;
                            }
                
                            .otp-gap {
                                width: 5px !important;
                            }
                        }
                
                        /* Small phone */
                        @media only screen and (max-width: 380px) {
                            .panel {
                                padding: 24px 16px !important;
                            }
                
                            .title {
                                font-size: 18px !important;
                            }
                
                            .otp-cell {
                                width: 36px !important;
                                height: 44px !important;
                                font-size: 17px !important;
                            }
                
                            .otp-dash {
                                width: 12px !important;
                                font-size: 13px !important;
                            }
                
                            .otp-gap {
                                width: 3px !important;
                            }
                        }
                    </style>
                </head>
                
                <body style="margin:0;padding:0;background-color:#ffffff;font-family:'Space Grotesk',Arial,sans-serif;">
                    <table width="100%" cellpadding="0" cellspacing="0" bgcolor="#ffffff"
                        style="background-color:#ffffff;padding:40px 16px;">
                        <tr>
                            <td align="center">
                                <table class="outer-table" width="640" cellpadding="0" cellspacing="0"
                                    style="max-width:640px;border-radius:20px;overflow:hidden;border:1px solid #252b35;box-shadow:0 30px 80px -20px rgba(0,0,0,0.7),0 0 60px -10px rgba(255,106,43,0.18);"
                                    class="card">
                                    <tr>
                                        <td class="panel" valign="top" bgcolor="#161a20"
                                            style="background-color:#161a20;padding:36px 40px;vertical-align:top;">
                
                                            <p
                                                style="margin:0 0 14px;font-family:'JetBrains Mono','Courier New',monospace;font-size:10px;text-transform:uppercase;letter-spacing:0.18em;color:#ff6a2b;">
                                                &#8212;&nbsp;Two&#8209;Factor Auth</p>
                
                                            <p class="title"
                                                style="margin:0 0 8px;font-family:'Space Grotesk',Arial,sans-serif;font-size:22px;font-weight:600;color:#f4f5f7;letter-spacing:-0.02em;">
                                                Verify your identity</p>
                
                                            <p style="margin:0 0 24px;font-size:13px;color:#8a909c;line-height:1.6;">Enter the code
                                                below to complete sign&#8209;in. Valid for <span
                                                    style="color:#f4f5f7;background-color:rgba(255,106,43,0.10);padding:1px 7px;border-radius:5px;border:1px solid rgba(255,106,43,0.2);font-family:'JetBrains Mono','Courier New',monospace;font-size:11px;">5&nbsp;min</span>
                                                and can only be used once.</p>
                
                                            <table class="otp-table" cellpadding="0" cellspacing="0" style="margin-bottom:20px;">
                                                <tr>
                                                    <td class="otp-cell" width="46" height="54" bgcolor="#111317"
                                                        style="background-color:#111317;border:1px solid #2f3641;border-radius:10px;text-align:center;vertical-align:middle;font-family:'JetBrains Mono','Courier New',monospace;font-size:22px;font-weight:600;color:#ffb14a;">
                                                        {{d1}}</td>
                                                    <td class="otp-gap" width="6"></td>
                                                    <td class="otp-cell" width="46" height="54" bgcolor="#111317"
                                                        style="background-color:#111317;border:1px solid #2f3641;border-radius:10px;text-align:center;vertical-align:middle;font-family:'JetBrains Mono','Courier New',monospace;font-size:22px;font-weight:600;color:#ffb14a;">
                                                        {{d2}}</td>
                                                    <td class="otp-gap" width="6"></td>
                                                    <td class="otp-cell" width="46" height="54" bgcolor="#111317"
                                                        style="background-color:#111317;border:1px solid #2f3641;border-radius:10px;text-align:center;vertical-align:middle;font-family:'JetBrains Mono','Courier New',monospace;font-size:22px;font-weight:600;color:#ffb14a;">
                                                        {{d3}}</td>
                                                    <td class="otp-dash" width="20"
                                                        style="text-align:center;font-family:'JetBrains Mono','Courier New',monospace;font-size:18px;color:#5a6271;">
                                                        &#8212;</td>
                                                    <td class="otp-cell" width="46" height="54" bgcolor="#111317"
                                                        style="background-color:#111317;border:1px solid #2f3641;border-radius:10px;text-align:center;vertical-align:middle;font-family:'JetBrains Mono','Courier New',monospace;font-size:22px;font-weight:600;color:#ffb14a;">
                                                        {{d4}}</td>
                                                    <td class="otp-gap" width="6"></td>
                                                    <td class="otp-cell" width="46" height="54" bgcolor="#111317"
                                                        style="background-color:#111317;border:1px solid #2f3641;border-radius:10px;text-align:center;vertical-align:middle;font-family:'JetBrains Mono','Courier New',monospace;font-size:22px;font-weight:600;color:#ffb14a;">
                                                        {{d5}}</td>
                                                    <td class="otp-gap" width="6"></td>
                                                    <td class="otp-cell" width="46" height="54" bgcolor="#111317"
                                                        style="background-color:#111317;border:1px solid #2f3641;border-radius:10px;text-align:center;vertical-align:middle;font-family:'JetBrains Mono','Courier New',monospace;font-size:22px;font-weight:600;color:#ffb14a;">
                                                        {{d6}}</td>
                                                </tr>
                                            </table>
                
                                            <p style="margin:0;font-size:11px;color:#5a6271;line-height:1.5;">If you did not request
                                                this code, you can safely ignore this email. Someone may have entered your address by
                                                mistake.</p>
                
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

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls, cancellationTokenSource.Token);
        await client.AuthenticateAsync(senderEmail, password, cancellationTokenSource.Token);
        await client.SendAsync(message, cancellationTokenSource.Token);
        await client.DisconnectAsync(true, cancellationTokenSource.Token);
    }
}
