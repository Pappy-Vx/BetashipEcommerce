using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Queries.Products.GetProductList;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Products;

public sealed class GetList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Products.GetList, HandleAsync)
           .WithName("GetProductList")
           .WithTags("Products")
           .WithSummary("Get paginated product list")
           .RequireRateLimiting(RateLimitPolicies.General)
           .Produces<PaginatedResult>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken,
        int pageNumber    = 1,
        int pageSize      = 10,
        string? category  = null,
        string? status    = null,
        string? search    = null)
    {
        var query = new GetProductListQuery
        {
            PageNumber     = pageNumber,
            PageSize       = pageSize,
            CategoryFilter = category,
            StatusFilter   = status,
            SearchTerm     = search
        };

        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private sealed record PaginatedResult;
}
