namespace ToDoTimeManager.WebApi.Extensions;

public static class StringExtensions
{
    public static string MaskAsEmail(this string email)
    {
        var atIndex = email.IndexOf('@');
        if (atIndex <= 1) return email;
        return $"{email[0]}***{email[atIndex..]}";
    }
}
