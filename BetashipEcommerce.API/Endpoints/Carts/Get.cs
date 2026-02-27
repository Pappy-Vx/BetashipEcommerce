using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Queries.Carts.GetCart;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Carts;

public sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Carts.Get, HandleAsync)
           .WithName("GetCart")
           .WithTags("Carts")
           .WithSummary("Get the active cart for a customer")
           .RequireAuthorization(PolicyNames.AuthenticatedUser)
           .RequireRateLimiting(RateLimitPolicies.General)
           .Produces<object>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid customerId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCartQuery(customerId), cancellationToken);

        return result is null
            ? Results.NotFound()
            : Results.Ok(result);
    }
}
