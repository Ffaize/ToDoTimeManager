using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using ToDoTimeManager.WebUI.Resources;
using ToDoTimeManager.WebUI.Utils;

namespace ToDoTimeManager.WebUI.Components.Base
{
    public abstract class BaseComponent : ComponentBase
    {
        [Inject] protected IStringLocalizer<Resource> Localizer { get; private set; } = null!;
        protected string SkeletonLoading => IsLoading ? "skeleton-loading" : string.Empty;
        protected bool IsLoading { get; private set; }

        protected async Task Loading(Func<Task> task, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IsLoading = true;
            await InvokeAsync(StateHasChanged);
            try
            {
                await task();
            }
            finally
            {
                IsLoading = false;
                await InvokeAsync(StateHasChanged);
            }
        }


        protected RenderFragment GetPageTitle(string nameOfPage)
        {
            var name = Localizer[PageTitleHelper.GetPageTitle(nameOfPage)].Value + " - TaskForge";
            return builder =>
            {
                builder.OpenComponent<PageTitle>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)((b) =>
                {
                    b.AddContent(2, name);
                }));
                builder.CloseComponent();
            };
        }

    }
}
