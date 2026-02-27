using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Orders.ConfirmOrder;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Orders;

public sealed class Confirm : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Orders.Confirm, HandleAsync)
           .WithName("ConfirmOrder")
           .WithTags("Orders")
           .WithSummary("Confirm a placed order")
           .RequireAuthorization(PolicyNames.AdminOnly)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        Guid orderId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ConfirmOrderCommand(orderId), cancellationToken);
        return result.ToApiResult();
    }
}
