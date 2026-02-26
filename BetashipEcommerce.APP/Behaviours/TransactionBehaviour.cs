using BetashipEcommerce.CORE.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.Behaviours;

/// <summary>
/// Wraps command handlers in a database transaction.
/// Only applies to commands (not queries) — follows CQS principle.
/// If the handler throws, the transaction is rolled back automatically.
/// </summary>
public sealed class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;

    public TransactionBehaviour(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Only wrap commands in transactions (not queries)
        if (!requestName.EndsWith("Command"))
            return await next();

        _logger.LogDebug("Beginning transaction for {RequestName}", requestName);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next();

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogDebug(" Transaction committed for {RequestName}", requestName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " Transaction rolled back for {RequestName}", requestName);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
