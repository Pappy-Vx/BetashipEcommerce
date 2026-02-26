using BetashipEcommerce.CORE.Orders.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.DomainEventHandlers;

/// <summary>
/// Handles order placed/created event.
/// Could trigger: email confirmation, inventory reservation, analytics.
/// </summary>
public sealed class OrderPlacedHandler : INotificationHandler<OrderPlacedNotification>
{
    private readonly ILogger<OrderPlacedHandler> _logger;

    public OrderPlacedHandler(ILogger<OrderPlacedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderPlacedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "📋 Order placed event handled: {OrderId} for customer {CustomerId}",
            notification.Event.OrderId.Value,
            notification.Event.CustomerId.Value);

        // TODO: Send order confirmation email
        // TODO: Trigger inventory reservation
        // TODO: Update customer analytics

        return Task.CompletedTask;
    }
}

public sealed record OrderPlacedNotification(OrderCreatedDomainEvent Event) : INotification;
