using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Queries.Customers.GetCustomerOrders;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Customers;

public sealed class GetOrders : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Customers.GetOrders, HandleAsync)
           .WithName("GetCustomerOrders")
           .WithTags("Customers")
           .WithSummary("Get all orders for a customer")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.General)
           .Produces<IReadOnlyList<object>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> HandleAsync(
        Guid customerId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCustomerOrdersQuery(customerId), cancellationToken);
        return Results.Ok(result);
    }
}
