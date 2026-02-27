using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Carts.ClearCart;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Carts;

public sealed class Clear : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Carts.Clear, HandleAsync)
           .WithName("ClearCart")
           .WithTags("Carts")
           .WithSummary("Remove all items from the customer's cart")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> HandleAsync(
        Guid customerId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ClearCartCommand(customerId), cancellationToken);
        return result.ToApiResult();
    }
}
