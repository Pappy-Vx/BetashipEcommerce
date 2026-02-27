using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;

namespace BetashipEcommerce.API.Endpoints.Admin;

public sealed class Dashboard : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Admin.Dashboard, HandleAsync)
           .WithName("AdminDashboard")
           .WithTags("Admin")
           .WithSummary("Get high-level platform metrics for the admin dashboard")
           .RequireAuthorization(PolicyNames.AdminOnly)
           .RequireRateLimiting(RateLimitPolicies.General)
           .Produces<DashboardSummary>(StatusCodes.Status200OK);
    }

    private static Task<IResult> HandleAsync(CancellationToken cancellationToken)
    {
        // TODO: Inject an analytics/reporting service and return real metrics
        var summary = new DashboardSummary(
            TotalOrders: 0,
            TotalRevenue: 0m,
            ActiveCustomers: 0,
            PublishedProducts: 0,
            PendingOrders: 0);

        return Task.FromResult(Results.Ok(summary));
    }
}

public sealed record DashboardSummary(
    int TotalOrders,
    decimal TotalRevenue,
    int ActiveCustomers,
    int PublishedProducts,
    int PendingOrders);
