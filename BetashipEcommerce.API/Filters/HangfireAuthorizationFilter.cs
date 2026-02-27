using Hangfire.Dashboard;

namespace BetashipEcommerce.API.Filters;

/// <summary>
/// Authorizes access to the Hangfire dashboard.
/// In development: always allows access.
/// In production: requires the user to be authenticated and hold the Admin role.
/// </summary>
public sealed class HangfireAuthorizationFilter(bool isDevelopment) : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        if (isDevelopment)
            return true;

        var httpContext = context.GetHttpContext();
        return httpContext.User.Identity?.IsAuthenticated == true &&
               httpContext.User.IsInRole("Admin");
    }
}
