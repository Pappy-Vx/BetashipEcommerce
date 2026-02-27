namespace BetashipEcommerce.API.Middleware;

/// <summary>
/// Wraps .NET 8 built-in rate limiting middleware with structured logging.
/// Actual rate limiting is configured in RateLimitConfiguration.cs.
/// </summary>
public static class RateLimitingMiddlewareExtensions
{
    public static WebApplication UseApiRateLimiting(this WebApplication app)
    {
        app.UseRateLimiter();

        app.Use(async (context, next) =>
        {
            await next();

            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                var logger = context.RequestServices
                    .GetRequiredService<ILogger<RateLimitingLog>>();

                logger.LogWarning(
                    "Rate limit exceeded: {Method} {Path} from {RemoteIp}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Connection.RemoteIpAddress);
            }
        });

        return app;
    }

    private sealed class RateLimitingLog;
}
