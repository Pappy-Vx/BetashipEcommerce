using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Queries.Orders.GetOrderById;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Orders;

public sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Orders.GetById, HandleAsync)
           .WithName("GetOrderById")
           .WithTags("Orders")
           .WithSummary("Get an order by ID")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.General)
           .Produces<OrderDto>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid orderId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetOrderByIdQuery(orderId), cancellationToken);

        return result is null
            ? Results.NotFound()
            : Results.Ok(result);
    }
}
