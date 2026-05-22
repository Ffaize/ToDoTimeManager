using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.WebUI.Components.Resources;

namespace ToDoTimeManager.WebUI.Pages;

public partial class FilesPage
{
    [Inject] private IStringLocalizer<Resource> Localizer { get; set; } = null!;
}
