using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;

namespace BetashipEcommerce.API.Endpoints.Admin;

public sealed class GetAnalytics : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Admin.Analytics, HandleAsync)
           .WithName("GetAnalytics")
           .WithTags("Admin")
           .WithSummary("Get platform analytics for a date range")
           .RequireAuthorization(PolicyNames.AdminOnly)
           .RequireRateLimiting(RateLimitPolicies.General)
           .Produces<AnalyticsSummary>(StatusCodes.Status200OK);
    }

    private static Task<IResult> HandleAsync(
        CancellationToken cancellationToken,
        DateTime? from = null,
        DateTime? to   = null)
    {
        // TODO: Inject IAnalyticsService and return real data
        var summary = new AnalyticsSummary(
            Period: $"{from:d} - {to:d}",
            TotalOrders: 0,
            TotalRevenue: 0m,
            NewCustomers: 0,
            TopProducts: Array.Empty<string>());

        return Task.FromResult(Results.Ok(summary));
    }
}

public sealed record AnalyticsSummary(
    string Period,
    int TotalOrders,
    decimal TotalRevenue,
    int NewCustomers,
    IReadOnlyList<string> TopProducts);
