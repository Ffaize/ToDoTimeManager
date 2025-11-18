namespace ToDoTimeManager.WebUI.Services.Implementations;

public class ToastsService
{
    public List<ToastModel> Messages { get; set; } = [];
    public Action? OnChange { get; set; }
    public void ShowToast(string message, bool isError)
    {
        Messages.Add(new ToastModel(message, isError));
        OnChange?.Invoke();
    }
}

public class ToastModel
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public bool IsError { get; set; }
    public ToastModel(string message, bool isError)
    {
        Id = Guid.NewGuid();
        Message = message;
        IsError = isError;
    }
}