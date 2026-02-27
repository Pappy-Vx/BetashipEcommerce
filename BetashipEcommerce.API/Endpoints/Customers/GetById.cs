using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Queries.Customers.GetCustomerById;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Customers;

public sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Customers.GetById, HandleAsync)
           .WithName("GetCustomerById")
           .WithTags("Customers")
           .WithSummary("Get customer profile by ID")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.General)
           .Produces<CustomerDto>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid customerId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCustomerByIdQuery(customerId), cancellationToken);

        return result is null
            ? Results.NotFound()
            : Results.Ok(result);
    }
}
