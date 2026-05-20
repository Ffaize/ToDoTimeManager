namespace ToDoTimeManager.Business.Utils.Interfaces;

public interface ITotpService
{
    string GenerateSecret();
    string BuildQrCodeUri(string email, string secret);
    string GenerateQrCodeBase64(string uri);
    bool   VerifyCode(string secret, string code);
}
