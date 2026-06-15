using System.Diagnostics;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var requestId = Activity.Current?.Id ?? context.TraceIdentifier;
            logger.LogError(exception, "Unhandled request error. RequestId: {RequestId}", requestId);

            var problem = CreateProblemDetails(context, exception, requestId);
            context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problem);
        }
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception, string requestId)
    {
        var status = exception is ValidationException
            ? StatusCodes.Status400BadRequest
            : StatusCodes.Status500InternalServerError;

        var problem = new ProblemDetails
        {
            Type = status == StatusCodes.Status400BadRequest
                ? "https://tools.ietf.org/html/rfc9110#section-15.5.1"
                : "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            Title = status == StatusCodes.Status400BadRequest ? "Validation error" : "Server error",
            Detail = environment.IsDevelopment()
                ? exception.Message
                : "An error occurred while processing the request.",
            Status = status,
            Instance = context.Request.Path
        };

        problem.Extensions["requestId"] = requestId;

        if (exception is ValidationException validationException)
        {
            problem.Extensions["errors"] = validationException.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());
        }

        return problem;
    }
}
