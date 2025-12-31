using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Orders.Entities
{
    public sealed class OrderItem : Entity<Guid>
    {
        public ProductId ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public Money UnitPrice { get; private set; }
        public Money TotalPrice { get; private set; }

        private OrderItem(
            Guid id,
            ProductId productId,
            string productName,
            int quantity,
            Money unitPrice)
            : base(id)
        {
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            CalculateTotalPrice();
        }

        private OrderItem() : base() { }

        public static OrderItem CreateWithPrice(
            ProductId productId,
            string productName,
            int quantity,
            Money unitPrice)
        {
            return new OrderItem(Guid.NewGuid(), productId, productName, quantity, unitPrice);
        }

        private void CalculateTotalPrice()
        {
            TotalPrice = Money.Create(UnitPrice.Amount * Quantity, UnitPrice.Currency);
        }
    }

}
