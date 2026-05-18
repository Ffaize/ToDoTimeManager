using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ToDoTimeManager.WebUI.Components.BaseComponents;

namespace ToDoTimeManager.WebUI.Pages;

public partial class TermsPage : BaseComponent
{
    [Inject] private IJSRuntime JS { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JS.InvokeVoidAsync("initLegalScroll");
    }
}
