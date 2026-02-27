using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.API.Services;

namespace BetashipEcommerce.API.Endpoints.Auth;

public sealed class Logout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Auth.Logout, HandleAsync)
           .WithName("Logout")
           .WithTags("Auth")
           .WithSummary("Invalidate the current session token")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.Auth)
           .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> HandleAsync(
        IAuthService authService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var token = httpContext.Request.Headers.Authorization
            .FirstOrDefault()
            ?.Replace("Bearer ", string.Empty);

        if (!string.IsNullOrWhiteSpace(token))
            await authService.LogoutAsync(token, cancellationToken);

        return Results.NoContent();
    }
}
