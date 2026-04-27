using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToDoTimeManager.WebApi.Exceptions;

namespace ToDoTimeManager.WebApi.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await next(context);
            stopwatch.Stop();
        }
        catch (ServiceException exception)
        {
            stopwatch.Stop();
            logger.LogWarning(exception, "Service exception caught. StatusCode: {StatusCode}. Elapsed: {Elapsed} ms",
                exception.StatusCode, stopwatch.ElapsedMilliseconds);

            var problemDetails =
                CreateProblemDetails(exception.StatusCode, GetTitle(exception.StatusCode), exception.Message);
            context.Response.StatusCode = exception.StatusCode;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            logger.LogError(exception, "An unhandled exception has occurred. Elapsed: {Elapsed} ms",
                stopwatch.ElapsedMilliseconds);

            var problemDetails =
                CreateProblemDetails(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

    private static string GetTitle(int statusCode)
    {
        return statusCode switch
        {
            400 => "Validation Error",
            403 => "Forbidden",
            404 => "Not Found",
            409 => "Conflict",
            var _ => "Error"
        };
    }

    private static ProblemDetails CreateProblemDetails(int status, string title, string? detail = null)
    {
        return new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };
    }
}