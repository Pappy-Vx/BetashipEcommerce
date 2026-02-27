using System.Diagnostics;

namespace BetashipEcommerce.API.Middleware;

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Items[CorrelationIdHeader]?.ToString() ?? "N/A";
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation(
            "[{CorrelationId}] {Method} {Path} started",
            correlationId,
            context.Request.Method,
            context.Request.Path);

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                "[{CorrelationId}] {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                correlationId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
