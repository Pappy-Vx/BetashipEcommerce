using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Orders.ShipOrder;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Orders;

public sealed class Ship : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Orders.Ship, HandleAsync)
           .WithName("ShipOrder")
           .WithTags("Orders")
           .WithSummary("Mark an order as shipped")
           .RequireAuthorization(PolicyNames.AdminOnly)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        Guid orderId,
        ShipOrderRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ShipOrderCommand(orderId, request.TrackingNumber),
            cancellationToken);

        return result.ToApiResult();
    }
}

public sealed record ShipOrderRequest(string? TrackingNumber);
