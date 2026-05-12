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
        var d4 = code.Length > 3 ? code[3].ToString() : "";
        var d5 = code.Length > 4 ? code[4].ToString() : "";
        var d6 = code.Length > 5 ? code[5].ToString() : "";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $"""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                  <meta charset="UTF-8" />
                  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                  <title>Verification Code — TaskForge</title>
                  <link href="https://fonts.googleapis.com/css2?family=Space+Grotesk:wght@500;600;700&family=JetBrains+Mono:wght@400;600&display=swap" rel="stylesheet" />
                </head>
                <body style="margin:0;padding:0;background-color:#0a0b0d;font-family:'Space Grotesk',Arial,sans-serif;">
                  <table width="100%" cellpadding="0" cellspacing="0" bgcolor="#0a0b0d" style="background-color:#0a0b0d;padding:40px 16px;">
                    <tr>
                      <td align="center">
                        <!-- Outer card -->
                        <table width="640" cellpadding="0" cellspacing="0" style="max-width:640px;border-radius:20px;overflow:hidden;border:1px solid #252b35;box-shadow:0 30px 80px -20px rgba(0,0,0,0.7),0 0 60px -10px rgba(255,106,43,0.18);">
                          <tr>

                            <!-- Left panel -->
                            <td width="210" valign="top" bgcolor="#0e1014" style="background-color:#0e1014;border-right:1px solid #252b35;padding:32px 24px;vertical-align:top;">

                              <!-- Brand row -->
                              <table cellpadding="0" cellspacing="0">
                                <tr>
                                  <td width="34" height="34" bgcolor="#ff6a2b" style="background-color:#ff6a2b;border-radius:8px;text-align:center;vertical-align:middle;">
                                    <span style="color:#ffffff;font-weight:700;font-size:17px;font-family:'Space Grotesk',Arial,sans-serif;line-height:34px;">T</span>
                                  </td>
                                  <td style="padding-left:10px;vertical-align:middle;">
                                    <span style="font-family:'Space Grotesk',Arial,sans-serif;font-size:16px;font-weight:700;color:#f4f5f7;letter-spacing:-0.01em;">Task<span style="color:#ff6a2b;">Forge</span></span>
                                  </td>
                                </tr>
                              </table>

                              <!-- Glow orb decoration -->
                              <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                  <td style="padding:32px 0 28px;text-align:center;">
                                    <table cellpadding="0" cellspacing="0" align="center">
                                      <tr>
                                        <td width="80" height="80" bgcolor="#1a1208" style="background-color:#1a1208;border-radius:50%;text-align:center;vertical-align:middle;border:1px solid #2f2010;">
                                          <span style="font-size:36px;line-height:80px;">⚒</span>
                                        </td>
                                      </tr>
                                    </table>
                                    <!-- Glow bar -->
                                    <table cellpadding="0" cellspacing="0" width="100%" style="margin-top:8px;">
                                      <tr>
                                        <td height="28" bgcolor="#130f09" style="background-color:#130f09;border-radius:0 0 8px 8px;opacity:0.7;"></td>
                                      </tr>
                                    </table>
                                  </td>
                                </tr>
                              </table>

                              <!-- Tagline -->
                              <p style="margin:0 0 6px;font-family:'Space Grotesk',Arial,sans-serif;font-size:17px;font-weight:500;color:#c9cdd4;line-height:1.3;letter-spacing:-0.01em;">Forge your<br/><span style="color:#ffb14a;">workflow.</span></p>
                              <p style="margin:0;font-size:11px;color:#8a909c;line-height:1.5;">Confirm your identity to keep building.</p>

                              <!-- Step ladder -->
                              <table cellpadding="0" cellspacing="0" width="100%" style="margin-top:24px;">
                                <tr>
                                  <td style="padding-bottom:10px;">
                                    <table cellpadding="0" cellspacing="0">
                                      <tr>
                                        <td width="22" height="22" bgcolor="#1f3a2c" style="background-color:#1f3a2c;border-radius:50%;border:1px solid rgba(34,197,94,0.4);text-align:center;vertical-align:middle;">
                                          <span style="font-family:'JetBrains Mono','Courier New',monospace;font-size:10px;color:#6dd896;line-height:22px;">✓</span>
                                        </td>
                                        <td style="padding-left:10px;">
                                          <span style="font-family:'JetBrains Mono','Courier New',monospace;font-size:10px;text-transform:uppercase;letter-spacing:0.04em;color:#c9cdd4;">Credentials</span>
                                        </td>
                                      </tr>
                                    </table>
                                  </td>
                                </tr>
                                <tr>
                                  <td>
                                    <table cellpadding="0" cellspacing="0">
                                      <tr>
                                        <td width="22" height="22" style="background:linear-gradient(180deg,#ff7b3d,#e85a1c);border-radius:50%;border:1px solid #ff6a2b;text-align:center;vertical-align:middle;box-shadow:0 0 0 3px rgba(255,106,43,0.15);">
                                          <span style="font-family:'JetBrains Mono','Courier New',monospace;font-size:10px;color:#ffffff;line-height:22px;">2</span>
                                        </td>
                                        <td style="padding-left:10px;">
                                          <span style="font-family:'JetBrains Mono','Courier New',monospace;font-size:10px;text-transform:uppercase;letter-spacing:0.04em;color:#ffb14a;">Verification</span>
                                        </td>
                                      </tr>
                                    </table>
                                  </td>
                                </tr>
                              </table>

                            </td>

                            <!-- Right panel -->
                            <td valign="top" bgcolor="#161a20" style="background-color:#161a20;padding:36px 32px;vertical-align:top;">

                              <!-- Eyebrow -->
                              <p style="margin:0 0 14px;font-family:'JetBrains Mono','Courier New',monospace;font-size:10px;text-transform:uppercase;letter-spacing:0.18em;color:#ff6a2b;">&#8212;&nbsp;Two&#8209;Factor Auth</p>

                              <!-- Title -->
                              <p style="margin:0 0 8px;font-family:'Space Grotesk',Arial,sans-serif;font-size:22px;font-weight:600;color:#f4f5f7;letter-spacing:-0.02em;">Verify your identity</p>

                              <!-- Subtitle -->
                              <p style="margin:0 0 24px;font-size:13px;color:#8a909c;line-height:1.6;">Enter the code below to complete sign-in. Valid for <span style="color:#f4f5f7;background-color:rgba(255,106,43,0.1);padding:1px 7px;border-radius:5px;border:1px solid rgba(255,106,43,0.2);font-family:'JetBrains Mono','Courier New',monospace;font-size:11px;">{lifetimeMinutes}&nbsp;min</span> and can only be used once.</p>

                              <!-- OTP code cells (3 — 3) -->
                              <table cellpadding="0" cellspacing="0" style="margin-bottom:22px;">
                                <tr>
                                  <td width="46" height="54" bgcolor="#111317" style="background-color:#111317;border:1px solid #2f3641;border-radius:10px;text-align:center;vertical-align:middle;font-family:'JetBrains Mono','Courier New',monospace;font-size:22px;font-weight:600;color:#ffb14a;">{d1}</td>
                                  <td width="6"></td>
                                  <td width="46" height="54" bgcolor="#111317" style="background-color:#111317;border:1px solid #2f3641;border-radius:10px;text-align:center;vertical-align:middle;font-family:'JetBrains Mono','Courier New',monospace;font-size:22px;font-weight:600;color:#ffb14a;">{d2}</td>
                                  <td width="6"></td>
                                  <td width="46" height="54" bgcolor="#111317" style="background-color:#111317;border:1px solid #2f3641;border-radius:10px;text-align:center;vertical-align:middle;font-family:'JetBrains Mono','Courier New',monospace;font-size:22px;font-weight:600;color:#ffb14a;">{d3}</td>
                                  <td width="20" style="text-align:center;font-family:'JetBrains Mono','Courier New',monospace;font-size:18px;color:#5a6271;">&#8212;</td>
                                  <td width="46" height="54" bgcolor="#111317" style="background-color:#111317;border:1px solid #2f3641;border-radius:10px;text-align:center;vertical-align:middle;font-family:'JetBrains Mono','Courier New',monospace;font-size:22px;font-weight:600;color:#ffb14a;">{d4}</td>
                                  <td width="6"></td>
                                  <td width="46" height="54" bgcolor="#111317" style="background-color:#111317;border:1px solid #2f3641;border-radius:10px;text-align:center;vertical-align:middle;font-family:'JetBrains Mono','Courier New',monospace;font-size:22px;font-weight:600;color:#ffb14a;">{d5}</td>
                                  <td width="6"></td>
                                  <td width="46" height="54" bgcolor="#111317" style="background-color:#111317;border:1px solid #2f3641;border-radius:10px;text-align:center;vertical-align:middle;font-family:'JetBrains Mono','Courier New',monospace;font-size:22px;font-weight:600;color:#ffb14a;">{d6}</td>
                                </tr>
                              </table>

                              <!-- Hint box -->
                              <table cellpadding="0" cellspacing="0" width="100%" style="margin-bottom:20px;">
                                <tr>
                                  <td bgcolor="#111317" style="background-color:rgba(255,255,255,0.02);border:1px solid #252b35;border-radius:10px;padding:12px 14px;">
                                    <p style="margin:0 0 3px;font-size:11px;font-weight:600;color:#c9cdd4;font-family:'Space Grotesk',Arial,sans-serif;">Not in your inbox?</p>
                                    <p style="margin:0;font-size:11px;color:#8a909c;line-height:1.5;font-family:'Space Grotesk',Arial,sans-serif;">Check your <span style="font-family:'JetBrains Mono','Courier New',monospace;color:#ffb14a;">Spam</span> or <span style="font-family:'JetBrains Mono','Courier New',monospace;color:#ffb14a;">Junk</span> folder if the email doesn't arrive.</p>
                                  </td>
                                </tr>
                              </table>

                              <!-- Ignore note -->
                              <p style="margin:0;font-size:11px;color:#5a6271;line-height:1.5;">If you did not request this code, you can safely ignore this email. Someone may have entered your address by mistake.</p>

                            </td>
                          </tr>

                          <!-- Footer -->
                          <tr>
                            <td colspan="2" bgcolor="#0e1014" style="background-color:#0e1014;border-top:1px solid #252b35;padding:14px 32px;text-align:center;">
                              <p style="margin:0;font-family:'JetBrains Mono','Courier New',monospace;font-size:10px;color:#5a6271;letter-spacing:0.08em;">&copy; {DateTime.UtcNow.Year} TASKFORGE &mdash; ALL RIGHTS RESERVED</p>
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
