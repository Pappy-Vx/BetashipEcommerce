using BetashipEcommerce.API.Constants;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace BetashipEcommerce.API.Configuration;

public static class RateLimitConfiguration
{
    public static IServiceCollection AddRateLimitingServices(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // General: 100 requests per minute per IP
            options.AddFixedWindowLimiter(RateLimitPolicies.General, opt =>
            {
                opt.PermitLimit      = 100;
                opt.Window           = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit       = 10;
            });

            // Auth endpoints: 10 requests per minute per IP (brute-force protection)
            options.AddFixedWindowLimiter(RateLimitPolicies.Auth, opt =>
            {
                opt.PermitLimit = 10;
                opt.Window      = TimeSpan.FromMinutes(1);
            });

            // Search: 30 requests per minute
            options.AddSlidingWindowLimiter(RateLimitPolicies.Search, opt =>
            {
                opt.PermitLimit          = 30;
                opt.Window               = TimeSpan.FromMinutes(1);
                opt.SegmentsPerWindow    = 6;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit           = 5;
            });

            // Write-heavy (cart, order mutations): 30 per minute
            options.AddFixedWindowLimiter(RateLimitPolicies.WriteHeavy, opt =>
            {
                opt.PermitLimit = 30;
                opt.Window      = TimeSpan.FromMinutes(1);
            });
        });

        return services;
    }
}
