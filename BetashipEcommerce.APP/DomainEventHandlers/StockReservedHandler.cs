using BetashipEcommerce.CORE.Inventory.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.DomainEventHandlers;

/// <summary>
/// Handles stock reserved events.
/// Could trigger: order inventory reservation confirmation, notifications, analytics.
/// </summary>
public sealed class StockReservedHandler : INotificationHandler<StockReservedNotification>
{
    private readonly ILogger<StockReservedHandler> _logger;

    public StockReservedHandler(ILogger<StockReservedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(StockReservedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Stock reserved event: Product {ProductId}, Qty {Quantity}, ReservationId {ReservationId}, For: {ReservedFor}",
            notification.Event.ProductId.Value,
            notification.Event.Quantity,
            notification.Event.ReservationId,
            notification.Event.ReservedFor);

        // TODO: Confirm inventory reservation on the order
        // TODO: Send stock reservation confirmation notification
        // TODO: Update inventory analytics

        return Task.CompletedTask;
    }
}

public sealed record StockReservedNotification(StockReservedDomainEvent Event) : INotification;
