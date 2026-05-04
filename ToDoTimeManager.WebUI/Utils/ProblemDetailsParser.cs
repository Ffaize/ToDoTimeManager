using System.Text.Json;

namespace ToDoTimeManager.WebUI.Utils;

public static class ProblemDetailsParser
{
    public static bool TryExtract(string content, out string? message)
    {
        message = null;

        if (!content.TrimStart().StartsWith("{"))
            return false;

        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
                return false;

            // ProblemDetails must have a numeric "status" field.
            if (!root.TryGetProperty("status", out var statusProp) ||
                statusProp.ValueKind != JsonValueKind.Number)
                return false;

            if (root.TryGetProperty("detail", out var detailProp) &&
                detailProp.ValueKind == JsonValueKind.String)
            {
                message = detailProp.GetString();
                return true;
            }

            // ValidationProblemDetails: { "errors": { "Field": ["msg", ...] } }
            if (root.TryGetProperty("errors", out var errorsProp) &&
                errorsProp.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in errorsProp.EnumerateObject())
                {
                    var arr = prop.Value;
                    if (arr.ValueKind == JsonValueKind.Array && arr.GetArrayLength() > 0 &&
                        arr[0].ValueKind == JsonValueKind.String)
                    {
                        message = arr[0].GetString();
                        return true;
                    }
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}
