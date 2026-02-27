using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BetashipEcommerce.API.Endpoints.Auth;

public sealed class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Auth.Login, HandleAsync)
           .WithName("Login")
           .WithTags("Auth")
           .WithSummary("Authenticate with email and password")
           .AllowAnonymous()
           .RequireRateLimiting(RateLimitPolicies.Auth)
           .Produces<AuthTokenResponse>(StatusCodes.Status200OK)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] LoginRequest request,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request.Email, request.Password, cancellationToken);

        return response is null
            ? Results.Unauthorized()
            : Results.Ok(response);
    }
}

public sealed record LoginRequest(string Email, string Password);
