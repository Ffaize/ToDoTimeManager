namespace ToDoTimeManager.Shared.DTOs.UserSettings;

public class TotpSetupResponseDto
{
    public string QrCodeBase64 { get; set; } = string.Empty;
    public string Secret       { get; set; } = string.Empty;
    public string AccountName  { get; set; } = string.Empty;
}
