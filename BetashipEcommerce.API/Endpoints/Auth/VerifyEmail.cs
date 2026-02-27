using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.API.Services;

namespace BetashipEcommerce.API.Endpoints.Auth;

public sealed class VerifyEmail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Auth.VerifyEmail, HandleAsync)
           .WithName("VerifyEmail")
           .WithTags("Auth")
           .WithSummary("Verify email address using the token sent by email")
           .AllowAnonymous()
           .RequireRateLimiting(RateLimitPolicies.Auth)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        string token,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Results.BadRequest(new { Message = "Verification token is required." });

        await authService.VerifyEmailAsync(token, cancellationToken);
        return Results.NoContent();
    }
}
