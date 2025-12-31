using BetashipEcommerce.CORE.Inventory.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Inventory.Events
{
    public sealed record InventoryItemCreatedDomainEvent(
    InventoryItemId InventoryItemId,
    ProductId ProductId,
    int InitialQuantity) : DomainEvent;

    public sealed record StockReservedDomainEvent(
        InventoryItemId InventoryItemId,
        ProductId ProductId,
        int Quantity,
        Guid ReservationId,
        string ReservedFor) : DomainEvent;

    public sealed record StockCommittedDomainEvent(
        InventoryItemId InventoryItemId,
        ProductId ProductId,
        int Quantity,
        Guid ReservationId) : DomainEvent;

    public sealed record StockReleasedDomainEvent(
        InventoryItemId InventoryItemId,
        ProductId ProductId,
        int Quantity,
        Guid ReservationId) : DomainEvent;

    public sealed record StockAddedDomainEvent(
        InventoryItemId InventoryItemId,
        ProductId ProductId,
        int Quantity,
        int NewTotalQuantity,
        string Reason) : DomainEvent;

    public sealed record StockRemovedDomainEvent(
        InventoryItemId InventoryItemId,
        ProductId ProductId,
        int Quantity,
        int NewTotalQuantity,
        string Reason) : DomainEvent;

    public sealed record ReorderLevelReachedDomainEvent(
        InventoryItemId InventoryItemId,
        ProductId ProductId,
        int CurrentQuantity,
        int ReorderLevel,
        int ReorderQuantity) : DomainEvent;
}
