using BetashipEcommerce.APP.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BetashipEcommerce.APP.Behaviours;

/// <summary>
/// MediatR pipeline behaviour that monitors request performance.
/// Logs a warning when any request exceeds the performance threshold (200ms by default).
/// Helps detect slow queries and commands early.
/// </summary>
public sealed class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int SlowRequestThresholdMs = 200;

    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> _logger;

    public PerformanceBehaviour(
        ICurrentUserService currentUserService,
        ILogger<PerformanceBehaviour<TRequest, TResponse>> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMs)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId?.ToString() ?? "anonymous";
            var userEmail = _currentUserService.Email ?? "unknown";

            _logger.LogWarning(
                "Slow request detected: {RequestName} ({ElapsedMs}ms) | User: {UserId} ({Email}) | Request: {@Request}",
                requestName,
                stopwatch.ElapsedMilliseconds,
                userId,
                userEmail,
                request);
        }

        return response;
    }
}
