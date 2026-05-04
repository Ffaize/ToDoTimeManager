using Microsoft.AspNetCore.Mvc;

namespace ToDoTimeManager.WebApi.Extensions;

public static class ProblemDetailsFactory
{
    public static string GetTitle(int statusCode) =>
        statusCode switch
        {
            400 => "Validation Error",
            403 => "Forbidden",
            404 => "Not Found",
            409 => "Conflict",
            _ => "Error"
        };

    public static ProblemDetails Create(int status, string title, string? detail = null) =>
        new()
        {
            Status = status,
            Title = title,
            Detail = detail
        };
}
