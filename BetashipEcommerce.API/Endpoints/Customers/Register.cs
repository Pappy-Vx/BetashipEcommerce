using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Customers.RegisterCustomer;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Customers;

public sealed class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Customers.Register, HandleAsync)
           .WithName("RegisterCustomer")
           .WithTags("Customers")
           .WithSummary("Register a new customer account")
           .RequireRateLimiting(RateLimitPolicies.Auth)
           .Produces<Guid>(StatusCodes.Status201Created)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        RegisterCustomerRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCustomerCommand(
            request.Email,
            request.FirstName,
            request.LastName);

        var result = await sender.Send(command, cancellationToken);

        return result.ToCreatedResult(
            "GetCustomerById",
            new { customerId = result.IsSuccess ? result.Value : Guid.Empty });
    }
}

public sealed record RegisterCustomerRequest(string Email, string FirstName, string LastName);
