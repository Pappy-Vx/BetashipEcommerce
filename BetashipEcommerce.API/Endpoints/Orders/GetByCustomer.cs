using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Queries.Orders.GetOrdersByCustomer;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Orders;

public sealed class GetByCustomer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Orders.GetByCustomer, HandleAsync)
           .WithName("GetOrdersByCustomer")
           .WithTags("Orders")
           .WithSummary("Get all orders for a specific customer")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.General)
           .Produces<IReadOnlyList<object>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> HandleAsync(
        Guid customerId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetOrdersByCustomerQuery(customerId), cancellationToken);
        return Results.Ok(result);
    }
}
