using BetashipEcommerce.API.Constants;
using BetashipEcommerce.API.Extensions;

namespace BetashipEcommerce.API.Endpoints.Admin;

public sealed class ManageUsers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Admin.ManageUsers)
                       .WithTags("Admin")
                       .RequireAuthorization(PolicyNames.AdminOnly)
                       .RequireRateLimiting(RateLimitPolicies.General);

        group.MapGet(string.Empty, ListUsersAsync)
             .WithName("ListAdminUsers")
             .WithSummary("List all registered users (paginated)")
             .Produces<IReadOnlyList<AdminUserEntry>>(StatusCodes.Status200OK);

        group.MapPut("{userId:guid}/roles", AssignRoleAsync)
             .WithName("AssignUserRole")
             .WithSummary("Assign or update roles for a user")
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static Task<IResult> ListUsersAsync(
        CancellationToken cancellationToken,
        int pageNumber = 1,
        int pageSize   = 20)
    {
        // TODO: Inject IUserAdminService and return real data
        return Task.FromResult(Results.Ok(Array.Empty<AdminUserEntry>()));
    }

    private static Task<IResult> AssignRoleAsync(
        Guid userId,
        AssignRoleRequest request,
        CancellationToken cancellationToken)
    {
        // TODO: Inject IUserAdminService and apply role change
        return Task.FromResult(Results.NoContent());
    }
}

public sealed record AdminUserEntry(Guid Id, string Email, string[] Roles, DateTime CreatedAt);
public sealed record AssignRoleRequest(string[] Roles);
