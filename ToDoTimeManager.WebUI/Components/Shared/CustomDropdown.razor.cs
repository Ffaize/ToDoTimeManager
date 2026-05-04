using Microsoft.AspNetCore.Components;

namespace ToDoTimeManager.WebUI.Components.Shared;

public partial class CustomDropdown<TItem>
{
    [Parameter, EditorRequired]
    public IEnumerable<TItem> Items { get; set; } = Enumerable.Empty<TItem>();

    [Parameter, EditorRequired]
    public Func<TItem, string> ItemLabel { get; set; } = null!;

    [Parameter] public string Placeholder { get; set; } = "Select…";

    [Parameter] public TItem? SelectedItem { get; set; }
    [Parameter] public EventCallback<TItem?> SelectedItemChanged { get; set; }

    [Parameter] public IList<TItem> SelectedItems { get; set; } = new List<TItem>();
    [Parameter] public EventCallback<IList<TItem>> SelectedItemsChanged { get; set; }

    [Parameter] public Func<TItem, RenderFragment?>? ItemIcon { get; set; }

    [Parameter] public bool Searchable { get; set; }
    [Parameter] public bool MultiSelect { get; set; }
    [Parameter] public string AdditionalCssClass { get; set; } = string.Empty;

    private bool _isOpen;
    private string _searchText = string.Empty;

    private IEnumerable<TItem> FilteredItems =>
        string.IsNullOrWhiteSpace(_searchText)
            ? Items
            : Items.Where(i => ItemLabel(i).Contains(_searchText, StringComparison.OrdinalIgnoreCase));

    private string GetTriggerLabel()
    {
        if (MultiSelect)
            return SelectedItems.Count == 0 ? Placeholder : string.Join(", ", SelectedItems.Select(ItemLabel));
        return SelectedItem is null ? Placeholder : ItemLabel(SelectedItem);
    }

    private bool IsSelected(TItem item) =>
        MultiSelect
            ? SelectedItems.Contains(item)
            : EqualityComparer<TItem>.Default.Equals(item, SelectedItem);

    private void ToggleDropdown()
    {
        _isOpen = !_isOpen;
        if (!_isOpen) _searchText = string.Empty;
    }

    private void CloseDropdown()
    {
        _isOpen = false;
        _searchText = string.Empty;
    }

    private async Task OnItemClick(TItem item)
    {
        if (MultiSelect)
        {
            var updated = new List<TItem>(SelectedItems);
            if (updated.Contains(item)) updated.Remove(item);
            else updated.Add(item);
            await SelectedItemsChanged.InvokeAsync(updated);
        }
        else
        {
            await SelectedItemChanged.InvokeAsync(item);
            CloseDropdown();
        }
    }
}
