namespace ToDoTimeManager.WebUI.Utils.PagesHelpers
{
    public static class PageTitleHelper
    {
        private static readonly Dictionary<string, string> PageTitles = new()
        {
            {"AuthPage", "Authorization"},
            {"TermsPage", "Terms of Service"},
            {"PrivacyPage", "Privacy Policy"},
            {"MainPage", "Dashboard"},
            {"TasksPage", "Tasks"},
            {"TimeLogsPage", "TimeLogs"},
            {"ProjectsPage", "Projects"},
            {"SettingsPage", "Settings"},
            {"SecurityPage", "Security"},
        };

        public static string GetPageTitle(string pageKey)
        {
            return PageTitles.GetValueOrDefault(pageKey, "ToDoTimeManager");
        }
    }
}
