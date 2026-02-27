using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Carts.RemoveFromCart;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Carts;

public sealed class RemoveItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Carts.RemoveItem, HandleAsync)
           .WithName("RemoveCartItem")
           .WithTags("Carts")
           .WithSummary("Remove a specific item from the cart")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid customerId,
        Guid productId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RemoveFromCartCommand(customerId, productId),
            cancellationToken);

        return result.ToApiResult();
    }
}
