using BetashipEcommerce.CORE.Inventory.Entities;
using BetashipEcommerce.CORE.Inventory.Enums;
using BetashipEcommerce.CORE.Inventory.Events;
using BetashipEcommerce.CORE.Inventory.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Inventory
{
    /// <summary>
    /// Manages stock levels and reservations for products
    /// Separate from Product to handle inventory concerns independently
    /// </summary>
    public sealed class InventoryItem : AggregateRoot<InventoryItemId>
    { 
        private readonly List<StockReservation> _reservations = new();

        public ProductId ProductId { get; private set; }
        public int AvailableQuantity { get; private set; }
        public int ReservedQuantity { get; private set; }
        public int TotalQuantity => AvailableQuantity + ReservedQuantity;
        public int ReorderLevel { get; private set; }
        public int ReorderQuantity { get; private set; }
        public DateTime LastRestocked { get; private set; }
        public IReadOnlyCollection<StockReservation> Reservations => _reservations.AsReadOnly();

        private InventoryItem(
            InventoryItemId id,
            ProductId productId,
            int initialQuantity,
            int reorderLevel,
            int reorderQuantity) : base(id)
        {
            ProductId = productId;
            AvailableQuantity = initialQuantity;
            ReservedQuantity = 0;
            ReorderLevel = reorderLevel;
            ReorderQuantity = reorderQuantity;
            LastRestocked = DateTime.UtcNow;
        }

        private InventoryItem() : base() { }

        public static Result<InventoryItem> Create(
            ProductId productId,
            int initialQuantity,
            int reorderLevel,
            int reorderQuantity)
        {
            if (initialQuantity < 0)
                return Result.Failure<InventoryItem>(InventoryErrors.InvalidQuantity);

            if (reorderLevel < 0)
                return Result.Failure<InventoryItem>(InventoryErrors.InvalidReorderLevel);

            if (reorderQuantity <= 0)
                return Result.Failure<InventoryItem>(InventoryErrors.InvalidReorderQuantity);

            var inventoryItem = new InventoryItem(
                new InventoryItemId(Guid.NewGuid()),
                productId,
                initialQuantity,
                reorderLevel,
                reorderQuantity);

            inventoryItem.RaiseDomainEvent(new InventoryItemCreatedDomainEvent(
                inventoryItem.Id,
                productId,
                initialQuantity));

            return Result.Success(inventoryItem);
        }

        /// <summary>
        /// Reserve stock for an order (when order is placed but not yet paid)
        /// </summary>
        public Result<StockReservation> ReserveStock(
            int quantity,
            string reservedFor,
            TimeSpan reservationDuration)
        {
            if (quantity <= 0)
                return Result.Failure<StockReservation>(InventoryErrors.InvalidQuantity);

            if (AvailableQuantity < quantity)
                return Result.Failure<StockReservation>(InventoryErrors.InsufficientStock);

            var reservation = StockReservation.Create(
                ProductId,
                quantity,
                reservedFor,
                DateTime.UtcNow.Add(reservationDuration));

            _reservations.Add(reservation);
            AvailableQuantity -= quantity;
            ReservedQuantity += quantity;

            RaiseDomainEvent(new StockReservedDomainEvent(
                Id,
                ProductId,
                quantity,
                reservation.Id,
                reservedFor));

            CheckReorderLevel();

            return Result.Success(reservation);
        }

        /// <summary>
        /// Commit reservation (when payment is completed)
        /// </summary>
        public Result CommitReservation(Guid reservationId)
        {
            var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId);
            if (reservation == null)
                return Result.Failure(InventoryErrors.ReservationNotFound);

            if (reservation.IsExpired)
                return Result.Failure(InventoryErrors.ReservationExpired);

            if (reservation.Status != ReservationStatus.Active)
                return Result.Failure(InventoryErrors.InvalidReservationStatus);

            reservation.Commit();
            ReservedQuantity -= reservation.Quantity;

            RaiseDomainEvent(new StockCommittedDomainEvent(
                Id,
                ProductId,
                reservation.Quantity,
                reservationId));

            return Result.Success();
        }

        /// <summary>
        /// Release reservation (when order is cancelled or payment fails)
        /// </summary>
        public Result ReleaseReservation(Guid reservationId)
        {
            var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId);
            if (reservation == null)
                return Result.Failure(InventoryErrors.ReservationNotFound);

            if (reservation.Status == ReservationStatus.Committed)
                return Result.Failure(InventoryErrors.CannotReleaseCommittedReservation);

            reservation.Release();
            AvailableQuantity += reservation.Quantity;
            ReservedQuantity -= reservation.Quantity;

            RaiseDomainEvent(new StockReleasedDomainEvent(
                Id,
                ProductId,
                reservation.Quantity,
                reservationId));

            return Result.Success();
        }

        /// <summary>
        /// Add stock (when inventory is restocked)
        /// </summary>
        public Result AddStock(int quantity, string reason)
        {
            if (quantity <= 0)
                return Result.Failure(InventoryErrors.InvalidQuantity);

            AvailableQuantity += quantity;
            LastRestocked = DateTime.UtcNow;

            RaiseDomainEvent(new StockAddedDomainEvent(
                Id,
                ProductId,
                quantity,
                TotalQuantity,
                reason));

            return Result.Success();
        }

        /// <summary>
        /// Remove stock (for damaged goods, theft, etc.)
        /// </summary>
        public Result RemoveStock(int quantity, string reason)
        {
            if (quantity <= 0)
                return Result.Failure(InventoryErrors.InvalidQuantity);

            if (AvailableQuantity < quantity)
                return Result.Failure(InventoryErrors.InsufficientStock);

            AvailableQuantity -= quantity;

            RaiseDomainEvent(new StockRemovedDomainEvent(
                Id,
                ProductId,
                quantity,
                TotalQuantity,
                reason));

            CheckReorderLevel();

            return Result.Success();
        }

        /// <summary>
        /// Clean up expired reservations
        /// </summary>
        public void CleanExpiredReservations()
        {
            var expiredReservations = _reservations
                .Where(r => r.IsExpired && r.Status == ReservationStatus.Active)
                .ToList();

            foreach (var reservation in expiredReservations)
            {
                ReleaseReservation(reservation.Id);
            }
        }

        private void CheckReorderLevel()
        {
            if (AvailableQuantity <= ReorderLevel)
            {
                RaiseDomainEvent(new ReorderLevelReachedDomainEvent(
                    Id,
                    ProductId,
                    AvailableQuantity,
                    ReorderLevel,
                    ReorderQuantity));
            }
        }
    }
}
