using BetashipEcommerce.CORE.Products.Events;
using BetashipEcommerce.CORE.SharedKernel.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.APP.DomainEventHandlers;

/// <summary>
/// Handles the ProductCreatedDomainEvent.
/// This could trigger notifications, update search indices, etc.
/// 
/// Note: Domain events are dispatched through the outbox pattern in the DAL layer.
/// This handler is invoked when the event is processed from the outbox.
/// </summary>
public sealed class ProductCreatedHandler : INotificationHandler<ProductCreatedNotification>
{
    private readonly ILogger<ProductCreatedHandler> _logger;

    public ProductCreatedHandler(ILogger<ProductCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ProductCreatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "📦 Product created event handled: {ProductId} - {ProductName}",
            notification.Event.ProductId.Value,
            notification.Event.ProductName);

        // TODO: Index product in search engine
        // TODO: Notify interested parties
        // TODO: Update analytics

        return Task.CompletedTask;
    }
}

/// <summary>
/// MediatR notification wrapper for domain events.
/// This bridges the gap between domain events (IDomainEvent) and MediatR notifications.
/// </summary>
public sealed record ProductCreatedNotification(ProductCreatedDomainEvent Event) : INotification;
