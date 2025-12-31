using BetashipEcommerce.CORE.Inventory.Enums;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Inventory.Entities
{
    public sealed class StockReservation : Entity<Guid>
    {
        public ProductId ProductId { get; private set; }
        public int Quantity { get; private set; }
        public string ReservedFor { get; private set; } // Order ID or Customer ID
        public DateTime ReservedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public ReservationStatus Status { get; private set; }
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;

        private StockReservation(
            Guid id,
            ProductId productId,
            int quantity,
            string reservedFor,
            DateTime expiresAt) : base(id)
        {
            ProductId = productId;
            Quantity = quantity;
            ReservedFor = reservedFor;
            ReservedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
            Status = ReservationStatus.Active;
        }

        private StockReservation() : base() { }

        public static StockReservation Create(
            ProductId productId,
            int quantity,
            string reservedFor,
            DateTime expiresAt)
        {
            return new StockReservation(
                Guid.NewGuid(),
                productId,
                quantity,
                reservedFor,
                expiresAt);
        }

        public void Commit()
        {
            Status = ReservationStatus.Committed;
        }

        public void Release()
        {
            Status = ReservationStatus.Released;
        }
    }

}
