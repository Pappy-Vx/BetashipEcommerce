using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BetashipEcommerce.API.Endpoints.Auth;

public sealed class RefreshToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Auth.RefreshToken, HandleAsync)
           .WithName("RefreshToken")
           .WithTags("Auth")
           .WithSummary("Refresh an expired access token using a valid refresh token")
           .AllowAnonymous()
           .RequireRateLimiting(RateLimitPolicies.Auth)
           .Produces<AuthTokenResponse>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] RefreshTokenRequest request,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        var response = await authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

        return response is null
            ? Results.Unauthorized()
            : Results.Ok(response);
    }
}

public sealed record RefreshTokenRequest(string RefreshToken);
