using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Orders.CancelOrder;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Orders;

public sealed class Cancel : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Orders.Cancel, HandleAsync)
           .WithName("CancelOrder")
           .WithTags("Orders")
           .WithSummary("Cancel an order")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        Guid orderId,
        CancelOrderRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CancelOrderCommand(orderId, request.Reason),
            cancellationToken);

        return result.ToApiResult();
    }
}

public sealed record CancelOrderRequest(string Reason);
