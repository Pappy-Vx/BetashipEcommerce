using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Carts.Entities
{
    public sealed class CartItem : Entity<Guid>
    {
        public ProductId ProductId { get; private set; }
        public int Quantity { get; private set; }
        public DateTime AddedAt { get; private set; }
        public DateTime LastModifiedAt { get; private set; }

        // Note: Price is NOT stored here
        // Always fetch current price from Product aggregate

        private CartItem(
            Guid id,
            ProductId productId,
            int quantity) : base(id)
        {
            ProductId = productId;
            Quantity = quantity;
            AddedAt = DateTime.UtcNow;
            LastModifiedAt = DateTime.UtcNow;
        }

        private CartItem() : base() { }

        public static CartItem Create(ProductId productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            return new CartItem(Guid.NewGuid(), productId, quantity);
        }

        public Result UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                return Result.Failure(CartErrors.InvalidQuantity);

            Quantity = newQuantity;
            LastModifiedAt = DateTime.UtcNow;

            return Result.Success();
        }
    }

}
