using BetashipEcommerce.APP.Common.Attributes;
using BetashipEcommerce.APP.Common.Exceptions;
using BetashipEcommerce.APP.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace BetashipEcommerce.APP.Behaviours;

/// <summary>
/// MediatR pipeline behaviour that enforces authorization on requests
/// decorated with [Authorize] or [Authorize(Roles = "...")]
/// </summary>
public sealed class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuthorizationBehaviour<TRequest, TResponse>> _logger;

    public AuthorizationBehaviour(
        ICurrentUserService currentUserService,
        ILogger<AuthorizationBehaviour<TRequest, TResponse>> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizeAttributes = request
            .GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToList();

        if (!authorizeAttributes.Any())
            return await next();

        // Must be authenticated
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
        {
            _logger.LogWarning(
                "Unauthorized request attempt for {RequestName}",
                typeof(TRequest).Name);

            throw new UnauthorizedException("You must be logged in to perform this action.");
        }

        // Check role-based authorization if roles are specified
        var requiredRoles = authorizeAttributes
            .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
            .SelectMany(a => a.Roles!.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(r => r.Trim())
            .ToList();

        if (requiredRoles.Any())
        {
            var userRoles = _currentUserService.Roles.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var hasRole = requiredRoles.Any(r => userRoles.Contains(r));

            if (!hasRole)
            {
                _logger.LogWarning(
                    "Forbidden: user {UserId} lacks required roles [{Roles}] for {RequestName}",
                    _currentUserService.UserId,
                    string.Join(", ", requiredRoles),
                    typeof(TRequest).Name);

                throw new ForbiddenException("You do not have permission to perform this action.");
            }
        }

        return await next();
    }
}
