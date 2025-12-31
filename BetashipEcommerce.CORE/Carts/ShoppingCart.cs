using BetashipEcommerce.CORE.Carts.Entities;
using BetashipEcommerce.CORE.Carts.Enum;
using BetashipEcommerce.CORE.Carts.Events;
using BetashipEcommerce.CORE.Carts.ValueObjects;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Carts
{
    /// <summary>
    /// Shopping cart that maintains current product prices
    /// Prices are always fetched from Product aggregate at checkout
    /// </summary>
    public sealed class ShoppingCart : AggregateRoot<ShoppingCartId>
    {
        private readonly List<CartItem> _items = new();

        public CustomerId CustomerId { get; private set; }
        public CartStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastModifiedAt { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

        // Cart items store ProductId reference, not price snapshot
        // Actual prices should be fetched from Product aggregate when needed
        public int TotalItems => _items.Sum(i => i.Quantity);

        private ShoppingCart(
            ShoppingCartId id,
            CustomerId customerId,
            TimeSpan? expirationDuration = null) : base(id)
        {
            CustomerId = customerId;
            Status = CartStatus.Active;
            CreatedAt = DateTime.UtcNow;
            LastModifiedAt = DateTime.UtcNow;

            // Default cart expiration: 30 days
            ExpiresAt = expirationDuration.HasValue
                ? DateTime.UtcNow.Add(expirationDuration.Value)
                : DateTime.UtcNow.AddDays(30);
        }

        private ShoppingCart() : base() { }

        public static Result<ShoppingCart> Create(
            CustomerId customerId,
            TimeSpan? expirationDuration = null)
        {
            var cart = new ShoppingCart(
                new ShoppingCartId(Guid.NewGuid()),
                customerId,
                expirationDuration);

            cart.RaiseDomainEvent(new ShoppingCartCreatedDomainEvent(cart.Id, customerId));

            return Result.Success(cart);
        }

        /// <summary>
        /// Add item to cart - stores ProductId reference only
        /// Price will be fetched from Product aggregate at checkout
        /// </summary>
        public Result AddItem(ProductId productId, int quantity)
        {
            if (Status != CartStatus.Active)
                return Result.Failure(CartErrors.CartNotActive);

            if (IsExpired())
                return Result.Failure(CartErrors.CartExpired);

            if (quantity <= 0)
                return Result.Failure(CartErrors.InvalidQuantity);

            var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                var updateResult = existingItem.UpdateQuantity(existingItem.Quantity + quantity);
                if (!updateResult.IsSuccess)
                    return updateResult;
            }
            else
            {
                var newItem = CartItem.Create(productId, quantity);
                _items.Add(newItem);
            }

            LastModifiedAt = DateTime.UtcNow;
            ExtendExpiration();

            RaiseDomainEvent(new ItemAddedToCartDomainEvent(
                Id,
                CustomerId,
                productId,
                quantity));

            return Result.Success();
        }

        /// <summary>
        /// Update item quantity in cart
        /// </summary>
        public Result UpdateItemQuantity(ProductId productId, int newQuantity)
        {
            if (Status != CartStatus.Active)
                return Result.Failure(CartErrors.CartNotActive);

            if (IsExpired())
                return Result.Failure(CartErrors.CartExpired);

            if (newQuantity < 0)
                return Result.Failure(CartErrors.InvalidQuantity);

            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                return Result.Failure(CartErrors.ItemNotFound);

            if (newQuantity == 0)
            {
                return RemoveItem(productId);
            }

            var result = item.UpdateQuantity(newQuantity);
            if (!result.IsSuccess)
                return result;

            LastModifiedAt = DateTime.UtcNow;
            ExtendExpiration();

            RaiseDomainEvent(new CartItemQuantityUpdatedDomainEvent(
                Id,
                productId,
                newQuantity));

            return Result.Success();
        }

        /// <summary>
        /// Remove item from cart
        /// </summary>
        public Result RemoveItem(ProductId productId)
        {
            if (Status != CartStatus.Active)
                return Result.Failure(CartErrors.CartNotActive);

            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                return Result.Failure(CartErrors.ItemNotFound);

            _items.Remove(item);
            LastModifiedAt = DateTime.UtcNow;

            RaiseDomainEvent(new ItemRemovedFromCartDomainEvent(
                Id,
                productId));

            return Result.Success();
        }

        /// <summary>
        /// Clear all items from cart
        /// </summary>
        public Result Clear()
        {
            if (Status != CartStatus.Active)
                return Result.Failure(CartErrors.CartNotActive);

            _items.Clear();
            LastModifiedAt = DateTime.UtcNow;

            RaiseDomainEvent(new ShoppingCartClearedDomainEvent(Id));

            return Result.Success();
        }

        /// <summary>
        /// Convert cart to order (called during checkout)
        /// Cart becomes inactive after conversion
        /// </summary>
        public Result ConvertToOrder()
        {
            if (Status != CartStatus.Active)
                return Result.Failure(CartErrors.CartNotActive);

            if (IsExpired())
                return Result.Failure(CartErrors.CartExpired);

            if (_items.Count == 0)
                return Result.Failure(CartErrors.EmptyCart);

            Status = CartStatus.CheckedOut;
            LastModifiedAt = DateTime.UtcNow;

            RaiseDomainEvent(new ShoppingCartConvertedToOrderDomainEvent(
                Id,
                CustomerId));

            return Result.Success();
        }

        /// <summary>
        /// Abandon cart (mark as abandoned for analytics)
        /// </summary>
        public Result Abandon()
        {
            if (Status == CartStatus.CheckedOut)
                return Result.Failure(CartErrors.CartAlreadyCheckedOut);

            Status = CartStatus.Abandoned;
            LastModifiedAt = DateTime.UtcNow;

            RaiseDomainEvent(new ShoppingCartAbandonedDomainEvent(
                Id,
                CustomerId,
                _items.Count,
                TotalItems));

            return Result.Success();
        }

        /// <summary>
        /// Check if cart has expired
        /// </summary>
        public bool IsExpired()
        {
            return ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
        }

        /// <summary>
        /// Extend cart expiration when user is active
        /// </summary>
        private void ExtendExpiration()
        {
            ExpiresAt = DateTime.UtcNow.AddDays(30);
        }

        /// <summary>
        /// Get cart items for order creation
        /// Note: Actual prices should be fetched from Product aggregate
        /// </summary>
        public IReadOnlyList<(ProductId ProductId, int Quantity)> GetItemsForOrder()
        {
            return _items.Select(i => (i.ProductId, i.Quantity)).ToList();
        }
    }

}
