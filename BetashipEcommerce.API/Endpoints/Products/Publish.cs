using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Commands.Products.PublishProduct;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Products;

public sealed class Publish : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Products.Publish, HandleAsync)
           .WithName("PublishProduct")
           .WithTags("Products")
           .WithSummary("Publish a product to make it visible to customers")
           .RequireAuthorization(PolicyNames.AdminOnly)
           .RequireRateLimiting(RateLimitPolicies.WriteHeavy)
           .Produces(StatusCodes.Status204NoContent)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        Guid productId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new PublishProductCommand(productId), cancellationToken);
        return result.ToApiResult();
    }
}
