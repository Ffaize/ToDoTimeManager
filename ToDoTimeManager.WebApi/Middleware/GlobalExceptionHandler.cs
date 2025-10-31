using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToDoTimeManager.WebApi.AdditionalComponents;

namespace ToDoTimeManager.WebApi.Middleware
{
    public class GlobalExceptionHandler(ILogger logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await next(context);
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    context.Response.StatusCode = StatusCodes.Status206PartialContent;
                    context.Response.Body = null;
                }


                stopwatch.Stop();
            }
            catch (CustomException exception)
            {
                stopwatch.Stop();
                logger.LogError(exception, $"Custom exception caught in middleware. Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");
                var problemDetails = CreateProblemDetails(StatusCodes.Status400BadRequest, "Client Error", exception.Message);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (Exception exception)
            {
                stopwatch.Stop();
                logger.LogError(exception, $"An unhandled exception has occurred. Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");

                var problemDetails = CreateProblemDetails(StatusCodes.Status204NoContent, exception.Message);

                context.Response.StatusCode = StatusCodes.Status204NoContent;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }

        private static ProblemDetails CreateProblemDetails(int status, string title, string detail = null)
        {
            return new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail
            };
        }
    }
}
