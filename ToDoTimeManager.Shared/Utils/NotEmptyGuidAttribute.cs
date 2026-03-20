using System.ComponentModel.DataAnnotations;

namespace ToDoTimeManager.Shared.Utils;

/// <summary>
/// Validates that a Guid value is not equal to Guid.Empty.
/// </summary>
public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null) return false;
        if (value is Guid guid) return guid != Guid.Empty;
        return false;
    }
}

