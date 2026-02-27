using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Carts.AddToCart;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Carts;

public sealed class AddItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Carts.AddItem, HandleAsync)
           .WithName("AddCartItem")
           .WithTags("Carts")
           .WithSummary("Add an item to the customer's cart")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        Guid customerId,
        AddCartItemRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new AddToCartCommand(customerId, request.ProductId, request.Quantity);
        var result  = await sender.Send(command, cancellationToken);
        return result.ToApiResult();
    }
}

public sealed record AddCartItemRequest(Guid ProductId, int Quantity);
