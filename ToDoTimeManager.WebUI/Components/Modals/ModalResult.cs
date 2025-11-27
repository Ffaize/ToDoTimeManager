namespace ToDoTimeManager.WebUI.Components.Modals
{
    public class ModalResult(bool show, bool needToFetchData, object? value = null, object? additionalValue = null)
    {
        public bool Show { get; set; } = show;
        public bool NeedToFetchData { get; set; } = needToFetchData;
        public object? Value { get; set; } = value;
        public object? AdditionalValue { get; set; } = additionalValue;
    }
}
