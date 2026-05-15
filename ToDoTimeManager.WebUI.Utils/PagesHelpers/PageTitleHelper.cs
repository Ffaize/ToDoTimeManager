namespace ToDoTimeManager.WebUI.Utils.PagesHelpers
{
    public static class PageTitleHelper
    {
        private static readonly Dictionary<string, string> PageTitles = new()
        {
            {"AuthPage", "Authorization"},
        };

        public static string GetPageTitle(string pageKey)
        {
            return PageTitles.GetValueOrDefault(pageKey, "ToDoTimeManager");
        }
    }
}
