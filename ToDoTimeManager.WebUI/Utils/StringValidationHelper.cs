using System.Text.RegularExpressions;

namespace ToDoTimeManager.WebUI.Utils;

public static class StringValidationHelper
{
    public static string DefaultValidation(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "This field can`t be empty or consist of spaces" : string.Empty;
    }

    public static string EmailValidation(string? email)
    {
        var result = DefaultValidation(email);
        if (!string.IsNullOrEmpty(result)) return result;
        var regex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email!, regex) ? string.Empty : "Invalid email format";
    }

    public static string PasswordValidation(string? password)
    {
        var result = DefaultValidation(password);
        if (!string.IsNullOrEmpty(result)) return result;
        if (password!.Length < 6) return "Password must be at least 6 characters long";
        if (!Regex.IsMatch(password, @"[A-Z]")) return "Password must contain at least one uppercase letter";
        if (!Regex.IsMatch(password, @"[a-z]")) return "Password must contain at least one lowercase letter";
        if (!Regex.IsMatch(password, @"[0-9]")) return "Password must contain at least one digit";
        return string.Empty;
    }

    public static string UsernameValidation(string? value)
    {
        var result = DefaultValidation(value);
        if (!string.IsNullOrEmpty(result)) return result;
        if (value!.Length < 3) return "Username must be at least 3 characters long";
        if (!Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$")) return "Username can only contain letters, digits and underscores";
        return string.Empty;
    }
    
    public static string ConfirmPasswordValidation(string? confirmPassword, string? password)
    {
        var result = DefaultValidation(confirmPassword);
        if (!string.IsNullOrEmpty(result)) return result;
        if (confirmPassword != password) return "Passwords do not match";
        return string.Empty;
    }

    public static string EmailOrUsernameValidation(string? logInParameter)
    {
        var result = DefaultValidation(logInParameter);
        if (!string.IsNullOrEmpty(result)) return result;
        return logInParameter!.Contains('@') ? EmailValidation(logInParameter) : UsernameValidation(logInParameter);
    }
}