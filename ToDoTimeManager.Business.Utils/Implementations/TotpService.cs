using OtpNet;
using QRCoder;
using ToDoTimeManager.Business.Utils.Interfaces;

namespace ToDoTimeManager.Business.Utils.Implementations;

public class TotpService : ITotpService
{
    private const string Issuer = "TaskForge";

    public string GenerateSecret()
        => Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20));

    public string BuildQrCodeUri(string email, string secret)
        => $"otpauth://totp/{Uri.EscapeDataString(Issuer)}:{Uri.EscapeDataString(email)}" +
           $"?secret={secret}&issuer={Uri.EscapeDataString(Issuer)}" +
           "&algorithm=SHA1&digits=6&period=30";

    public string GenerateQrCodeBase64(string uri)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrData);
        var pngBytes = qrCode.GetGraphic(20);
        return Convert.ToBase64String(pngBytes);
    }

    public bool VerifyCode(string secret, string code)
    {
        if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(code))
            return false;
        try
        {
            var totp = new Totp(Base32Encoding.ToBytes(secret));
            return totp.VerifyTotp(code, out _, new VerificationWindow(1, 1));
        }
        catch
        {
            return false;
        }
    }
}
