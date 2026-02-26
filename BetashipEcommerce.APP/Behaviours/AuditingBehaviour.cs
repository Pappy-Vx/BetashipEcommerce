using BetashipEcommerce.APP.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BetashipEcommerce.APP.Behaviours;

/// <summary>
/// MediatR pipeline behaviour that records audit information for every command.
/// Queries are skipped — only state-changing commands are audited.
/// </summary>
public sealed class AuditingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuditingBehaviour<TRequest, TResponse>> _logger;

    public AuditingBehaviour(
        ICurrentUserService currentUserService,
        ILogger<AuditingBehaviour<TRequest, TResponse>> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Only audit commands (skip queries)
        if (!requestName.EndsWith("Command", StringComparison.OrdinalIgnoreCase))
            return await next();

        var userId = _currentUserService.UserId?.ToString() ?? "anonymous";
        var userEmail = _currentUserService.Email ?? "unknown";

        _logger.LogInformation(
            "[AUDIT] Command: {CommandName} | User: {UserId} ({Email}) | At: {Timestamp}",
            requestName,
            userId,
            userEmail,
            DateTime.UtcNow);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            stopwatch.Stop();

            _logger.LogInformation(
                "[AUDIT] Command: {CommandName} | Status: Succeeded | Duration: {ElapsedMs}ms | User: {UserId}",
                requestName,
                stopwatch.ElapsedMilliseconds,
                userId);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "[AUDIT] Command: {CommandName} | Status: Failed | Duration: {ElapsedMs}ms | User: {UserId} | Error: {Error}",
                requestName,
                stopwatch.ElapsedMilliseconds,
                userId,
                ex.Message);

            throw;
        }
    }
}
