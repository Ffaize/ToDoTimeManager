namespace ToDoTimeManager.WebUI.Utils.PagesHelpers
{
    public static class PageTitleHelper
    {
        private static readonly Dictionary<string, string> PageTitles = new()
        {
            {"AuthPage", "Authorization"},
            {"TermsPage", "Terms of Service"},
            {"PrivacyPage", "Privacy Policy"},
        };

        public static string GetPageTitle(string pageKey)
        {
            return PageTitles.GetValueOrDefault(pageKey, "ToDoTimeManager");
        }
    }
}
