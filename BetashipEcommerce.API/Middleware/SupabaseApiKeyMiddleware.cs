namespace BetashipEcommerce.API.Middleware;

/// <summary>
/// Validates the Supabase anon API key on incoming requests for public endpoints.
/// Endpoints decorated with [SupabaseApiKey] require a valid anon key in the header.
/// </summary>
public sealed class SupabaseApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private const string ApiKeyHeader   = "apikey";
    private const string ApiKeySection  = "Supabase:AnonKey";

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        var requiresApiKey = endpoint?
            .Metadata
            .GetMetadata<RequiresSupabaseApiKeyAttribute>() is not null;

        if (requiresApiKey)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var providedKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing Supabase API key.");
                return;
            }

            var expectedKey = configuration[ApiKeySection];
            if (string.IsNullOrWhiteSpace(expectedKey) || providedKey != expectedKey)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid Supabase API key.");
                return;
            }
        }

        await next(context);
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequiresSupabaseApiKeyAttribute : Attribute;
