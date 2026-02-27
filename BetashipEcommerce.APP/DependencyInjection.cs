using BetashipEcommerce.APP.Behaviours;
using BetashipEcommerce.APP.Common.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BetashipEcommerce.APP;

/// <summary>
/// Application Layer DI registration.
/// Registers MediatR, FluentValidation, Pipeline Behaviours, AutoMapper, and common services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register MediatR handlers from this assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            // Pipeline Behaviours (order matters!)
            // 1. Logging: Log every request
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

            // 2. Authorization: Enforce access control before processing
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));

            // 3. Validation: Validate before processing
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            // 4. Performance: Monitor slow requests
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

            // 5. Auditing: Record command audit trail
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuditingBehaviour<,>));

            // 6. Transaction: Wrap commands in DB transactions
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
        });

        // Register all FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(assembly);

        // Register AutoMapper — scans this assembly for profiles (picks up MappingProfile)
        services.AddAutoMapper(assembly);

        // Register common services
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
