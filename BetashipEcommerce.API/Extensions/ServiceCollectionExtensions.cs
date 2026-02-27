using BetashipEcommerce.API.Configuration;
using BetashipEcommerce.API.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BetashipEcommerce.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSwaggerServices(configuration);
        services.AddCorsServices(configuration);
        services.AddJwtAuthentication(configuration);
        services.AddAuthorizationPolicies();
        services.AddRateLimitingServices();

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are not configured.");

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Authority is only set when using an external OIDC provider (e.g. Supabase in prod).
                // In dev we use symmetric key validation directly, so authority is left unset to
                // prevent the middleware from attempting OIDC discovery against a non-existent endpoint.
                if (!string.IsNullOrWhiteSpace(jwtSettings.Authority) &&
                    !jwtSettings.Authority.StartsWith("https://localhost"))
                {
                    options.Authority = jwtSettings.Authority;
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidIssuer              = jwtSettings.Issuer,
                    ValidateAudience         = true,
                    ValidAudience            = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateLifetime = true,
                    ClockSkew        = TimeSpan.FromSeconds(30)
                };
            });

        return services;
    }

    private static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.AdminOnly,
                policy => policy.RequireRole("Admin"));

            options.AddPolicy(PolicyNames.AuthenticatedUser,
                policy => policy.RequireAuthenticatedUser());

            options.AddPolicy(PolicyNames.CustomerOrAdmin,
                policy => policy.RequireRole("Customer", "Admin"));
        });

        return services;
    }
}
