using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace BetashipEcommerce.API.Middleware;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger,
    IHostEnvironment environment)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Items["X-Correlation-ID"]?.ToString();

        logger.LogError(
            exception,
            "[{CorrelationId}] Unhandled exception on {Method} {Path}",
            correlationId,
            context.Request.Method,
            context.Request.Path);

        var (statusCode, title) = exception switch
        {
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            ArgumentNullException       => (HttpStatusCode.BadRequest,   "Bad Request"),
            ArgumentException           => (HttpStatusCode.BadRequest,   "Bad Request"),
            InvalidOperationException   => (HttpStatusCode.BadRequest,   "Invalid Operation"),
            _                           => (HttpStatusCode.InternalServerError, "Internal Server Error")
        };

        var problem = new ProblemDetails
        {
            Status   = (int)statusCode,
            Title    = title,
            Detail   = environment.IsDevelopment() ? exception.Message : "An unexpected error occurred.",
            Instance = context.Request.Path
        };

        problem.Extensions["correlationId"] = correlationId;

        if (environment.IsDevelopment())
            problem.Extensions["stackTrace"] = exception.StackTrace;

        context.Response.StatusCode  = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
    }
}
