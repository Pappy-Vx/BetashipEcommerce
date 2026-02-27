using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.API.Services;
using BetashipEcommerce.APP.Commands.Customers.RegisterCustomer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BetashipEcommerce.API.Endpoints.Auth;

public sealed class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Auth.Register, HandleAsync)
           .WithName("AuthRegister")
           .WithTags("Auth")
           .WithSummary("Register a new account (creates auth identity + customer profile)")
           .AllowAnonymous()
           .RequireRateLimiting(RateLimitPolicies.Auth)
           .Produces<AuthTokenResponse>(StatusCodes.Status201Created)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] AuthRegisterRequest request,
        IAuthService authService,
        ISender sender,
        CancellationToken cancellationToken)
    {
        // 1. Create the auth identity in Supabase
        var authResponse = await authService.RegisterAsync(request.Email, request.Password, cancellationToken);
        if (authResponse is null)
            return Results.Conflict(new { Message = "An account with this email already exists." });

        // 2. Create the customer profile in the domain
        var command = new RegisterCustomerCommand(request.Email, request.FirstName, request.LastName);
        await sender.Send(command, cancellationToken);

        return Results.Created(ApiRoutes.Auth.Login, authResponse);
    }
}

public sealed record AuthRegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName);
