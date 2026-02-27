namespace BetashipEcommerce.API.Filters;

/// <summary>
/// Endpoint filter that enforces authentication before reaching the handler.
/// Prefer using .RequireAuthorization() on the endpoint instead of this filter.
/// Use this when you need custom authorization logic beyond standard policies.
/// </summary>
public sealed class AuthorizationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var user = context.HttpContext.User;

        if (user.Identity?.IsAuthenticated != true)
            return Results.Unauthorized();

        return await next(context);
    }
}
