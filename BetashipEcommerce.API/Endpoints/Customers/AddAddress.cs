using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Customers.AddCustomerAddress;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Customers;

public sealed class AddAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Customers.AddAddress, HandleAsync)
           .WithName("AddCustomerAddress")
           .WithTags("Customers")
           .WithSummary("Add a new address to customer profile")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces<Guid>(StatusCodes.Status201Created)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid customerId,
        AddAddressRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new AddCustomerAddressCommand(
            customerId,
            request.Label,
            request.Street,
            request.City,
            request.State,
            request.Country,
            request.PostalCode,
            request.IsDefault);

        var result = await sender.Send(command, cancellationToken);
        return result.ToApiResult<Guid>();
    }
}

public sealed record AddAddressRequest(
    string Label,
    string Street,
    string City,
    string State,
    string Country,
    string PostalCode,
    bool IsDefault);
