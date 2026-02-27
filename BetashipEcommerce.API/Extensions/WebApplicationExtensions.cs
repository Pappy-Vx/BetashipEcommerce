using System.Reflection;

namespace BetashipEcommerce.API.Extensions;

/// <summary>
/// Marker interface for minimal API endpoint registrations.
/// Each endpoint class implements this to self-register its routes.
/// </summary>
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}

public static class WebApplicationExtensions
{
    /// <summary>
    /// Discovers all IEndpoint implementations in the given assemblies and maps their routes.
    /// </summary>
    public static WebApplication MapEndpoints(this WebApplication app, params Assembly[] assemblies)
    {
        var endpointTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsAbstract: false, IsInterface: false } &&
                        t.IsAssignableTo(typeof(IEndpoint)));

        foreach (var type in endpointTypes)
        {
            if (Activator.CreateInstance(type) is IEndpoint endpoint)
                endpoint.MapEndpoint(app);
        }

        return app;
    }
}
