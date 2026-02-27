using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Carts.UpdateCartItem;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Carts;

public sealed class UpdateItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Carts.UpdateItem, HandleAsync)
           .WithName("UpdateCartItem")
           .WithTags("Carts")
           .WithSummary("Update the quantity of an item in the cart")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid customerId,
        Guid productId,
        UpdateCartItemRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCartItemCommand(customerId, productId, request.NewQuantity);
        var result  = await sender.Send(command, cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record UpdateCartItemRequest(int NewQuantity);
