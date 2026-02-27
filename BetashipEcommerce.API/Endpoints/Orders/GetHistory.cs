using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;
using BetashipEcommerce.APP.Queries.Orders.GetOrderHistory;
using MediatR;

namespace BetashipEcommerce.API.Endpoints.Orders;

public sealed class GetHistory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Orders.GetHistory, HandleAsync)
           .WithName("GetOrderHistory")
           .WithTags("Orders")
           .WithSummary("Get paginated order history with optional date range filter")
           .RequireAuthorization(PolicyNames.AdminOnly)
           .RequireRateLimiting(RateLimitPolicies.General)
           .Produces<object>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken,
        DateTime? from  = null,
        DateTime? to    = null,
        int pageNumber  = 1,
        int pageSize    = 20)
    {
        var query  = new GetOrderHistoryQuery(from, to, pageNumber, pageSize);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }
}
