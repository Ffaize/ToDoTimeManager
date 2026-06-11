namespace ToDoTimeManager.WebUI.Components.Shared.Navigation;

public partial class NotificationsBell
{
    private bool _isOpen;

    private void Toggle()
    {
        _isOpen = !_isOpen;
    }

    private void Close()
    {
        _isOpen = false;
    }
}
