using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;

namespace BetashipEcommerce.API.Endpoints.Admin;

public sealed class GetAuditLogs : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Admin.AuditLogs, HandleAsync)
           .WithName("GetAuditLogs")
           .WithTags("Admin")
           .WithSummary("Retrieve paginated audit log entries")
           .RequireAuthorization(PolicyNames.AdminOnly)
           .RequireRateLimiting(RateLimitPolicies.General)
           .Produces<IReadOnlyList<AuditLogEntry>>(StatusCodes.Status200OK);
    }

    private static Task<IResult> HandleAsync(
        CancellationToken cancellationToken,
        int pageNumber = 1,
        int pageSize   = 50,
        string? userId = null,
        string? action = null)
    {
        // TODO: Inject IAuditLogService and return real audit entries
        return Task.FromResult(Results.Ok(Array.Empty<AuditLogEntry>()));
    }
}

public sealed record AuditLogEntry(
    Guid Id,
    string UserId,
    string Action,
    string EntityType,
    string EntityId,
    DateTime Timestamp,
    bool Succeeded);
