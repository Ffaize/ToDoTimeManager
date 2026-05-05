using ToDoTimeManager.WebUI.Pages;

namespace ToDoTimeManager.WebUI.Utils
{
    public static class PageTitleHelper
    {
        private static readonly Dictionary<string, string> PageTitles = new()
        {
            {nameof(AuthPage), "Authorization"},
        };

        public static string GetPageTitle(string pageKey)
        {
            return PageTitles.GetValueOrDefault(pageKey, "ToDoTimeManager");
        }
    }
}
