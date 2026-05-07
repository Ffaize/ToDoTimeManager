using System.Diagnostics;
using ToDoTimeManager.WebApi.Exceptions;
using ToDoTimeManager.WebApi.Extensions;

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
                ProblemDetailsFactory.Create(exception.StatusCode, ProblemDetailsFactory.GetTitle(exception.StatusCode), exception.Message);
            context.Response.StatusCode = exception.StatusCode;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            logger.LogError(exception, "An unhandled exception has occurred. Elapsed: {Elapsed} ms",
                stopwatch.ElapsedMilliseconds);

            var problemDetails =
                ProblemDetailsFactory.Create(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}