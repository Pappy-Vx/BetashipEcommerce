using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Queries.Products.SearchProducts;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Products;

public sealed class Search : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Products.Search, HandleAsync)
           .WithName("SearchProducts")
           .WithTags("Products")
           .WithSummary("Search products by name, description, or SKU")
           .RequireRateLimiting(RateLimitPolicies.Search)
           .Produces<object>(StatusCodes.Status200OK)
           .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken,
        string q          = "",
        int pageNumber    = 1,
        int pageSize      = 20)
    {
        var query  = new SearchProductsQuery(q, pageNumber, pageSize);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }
}
