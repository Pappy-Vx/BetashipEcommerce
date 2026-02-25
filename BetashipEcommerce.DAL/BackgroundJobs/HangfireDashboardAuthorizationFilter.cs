using Hangfire.Dashboard;

namespace BetashipEcommerce.DAL.BackgroundJobs
{
    /// <summary>
    /// Authorization filter for Hangfire Dashboard.
    /// 
    /// In Development: Allows all access (no auth required)
    /// In Production: Restrict to admin users only
    /// 
    /// Configure in Program.cs:
    ///   app.UseHangfireDashboard("/hangfire", new DashboardOptions
    ///   {
    ///       Authorization = new[] { new HangfireDashboardAuthorizationFilter(env.IsDevelopment()) }
    ///   });
    /// </summary>
    public sealed class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly bool _allowAll;

        public HangfireDashboardAuthorizationFilter(bool allowAll = false)
        {
            _allowAll = allowAll;
        }

        public bool Authorize(DashboardContext context)
        {
            // In development, allow all access
            if (_allowAll)
                return true;

            // In production, require authentication
            var httpContext = context.GetHttpContext();
            return httpContext.User.Identity?.IsAuthenticated == true
                && httpContext.User.IsInRole("Admin");
        }
    }
}
