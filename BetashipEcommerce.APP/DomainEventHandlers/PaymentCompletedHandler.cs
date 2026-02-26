using BetashipEcommerce.CORE.Payments.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.DomainEventHandlers;

/// <summary>
/// Handles payment completed event.
/// Triggers order confirmation flow and inventory commitment.
/// </summary>
public sealed class PaymentCompletedHandler : INotificationHandler<PaymentCompletedNotification>
{
    private readonly ILogger<PaymentCompletedHandler> _logger;

    public PaymentCompletedHandler(ILogger<PaymentCompletedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(PaymentCompletedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            " Payment completed event handled: {PaymentId} for order {OrderId}. Amount: {Amount}",
            notification.Event.PaymentId.Value,
            notification.Event.OrderId.Value,
            notification.Event.Amount.Amount);

        // TODO: Mark order as paid
        // TODO: Commit inventory reservation
        // TODO: Send payment receipt email
        // TODO: Update financial reports

        return Task.CompletedTask;
    }
}

public sealed record PaymentCompletedNotification(PaymentCompletedDomainEvent Event) : INotification;
