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
        public int Quantity { get; private set; }
        public Money UnitPrice { get; private set; }
        public Money TotalPrice { get; private set; }

        private OrderItem(Guid id, ProductId productId, int quantity, Money unitPrice)
            : base(id)
        {
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            CalculateTotalPrice();
        }

        private OrderItem() : base() { }

        public static OrderItem Create(ProductId productId, int quantity, Money unitPrice)
        {
            return new OrderItem(Guid.NewGuid(), productId, quantity, unitPrice);
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

            Quantity = newQuantity;
            CalculateTotalPrice();
        }

        private void CalculateTotalPrice()
        {
            TotalPrice = Money.Create(UnitPrice.Amount * Quantity, UnitPrice.Currency);
        }
    }

}
