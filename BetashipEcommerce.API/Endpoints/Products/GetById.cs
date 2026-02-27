using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Queries.Products.GetProductById;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Products;

public sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Products.GetById, HandleAsync)
           .WithName("GetProductById")
           .WithTags("Products")
           .WithSummary("Get a product by ID")
           .RequireRateLimiting(RateLimitPolicies.General)
           .Produces<ProductDto>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid productId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductByIdQuery(productId), cancellationToken);

        return result is null
            ? Results.NotFound()
            : Results.Ok(result);
    }
}
