using Newtonsoft.Json.Linq;
using System.Globalization;

namespace ToDoTimeManager.WebUI.Utils
{
    public static class CultureInfoHelper
    {
        public static string GetCultureInfoDisplayName(this CultureInfo cultureInfo)
        {
            var name = cultureInfo.NativeName;
            if (string.IsNullOrEmpty(name))
                return name;

            return char.ToUpper(name[0], CultureInfo.CurrentCulture) + name[1..];
        }
    }
}
