namespace BetashipEcommerce.API.Configuration;

public static class CorsConfiguration
{
    private const string DevPolicyName  = "Development";
    private const string ProdPolicyName = "Production";

    public static IServiceCollection AddCorsServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy(DevPolicyName, policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod());

            options.AddPolicy(ProdPolicyName, policy =>
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
                      .AllowCredentials());
        });

        return services;
    }

    public static WebApplication UseCorsPolicy(this WebApplication app)
    {
        var policyName = app.Environment.IsDevelopment() ? DevPolicyName : ProdPolicyName;
        app.UseCors(policyName);
        return app;
    }
}
