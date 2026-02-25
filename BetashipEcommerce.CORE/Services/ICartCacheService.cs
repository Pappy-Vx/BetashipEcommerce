using BetashipEcommerce.CORE.Carts.ValueObjects;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;

namespace BetashipEcommerce.CORE.Services
{
    /// <summary>
    /// Redis-backed cart cache service for blazing-fast active cart operations.
    /// Part of the Hybrid (Redis + Database) cart strategy:
    /// - Active carts live in Redis (microsecond access)
    /// - At checkout, cart moves to Database (permanent record)
    /// - Abandoned carts (24+ hours) get synced to Database via background job
    /// </summary>
    public interface ICartCacheService
    {
        /// <summary>
        /// Get the active cart for a customer from Redis cache.
        /// Returns null if no active cart exists in cache.
        /// </summary>
        Task<CartCacheDto?> GetCartAsync(CustomerId customerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add an item to the customer's cart in Redis.
        /// Creates the cart if it doesn't exist.
        /// </summary>
        Task AddItemAsync(CustomerId customerId, ProductId productId, int quantity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update the quantity of an existing item in the cart.
        /// </summary>
        Task<bool> UpdateItemQuantityAsync(CustomerId customerId, ProductId productId, int newQuantity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove a specific item from the cart.
        /// </summary>
        Task<bool> RemoveItemAsync(CustomerId customerId, ProductId productId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Clear all items from the cart.
        /// </summary>
        Task ClearCartAsync(CustomerId customerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove the entire cart from Redis (typically after checkout).
        /// </summary>
        Task DeleteCartAsync(CustomerId customerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if customer has an active cart in Redis.
        /// </summary>
        Task<bool> CartExistsAsync(CustomerId customerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all cart keys that have been inactive for longer than the specified duration.
        /// Used by background jobs to identify abandoned carts for database sync.
        /// </summary>
        Task<IReadOnlyList<string>> GetAbandonedCartKeysAsync(TimeSpan inactiveDuration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a cart by its Redis key (used by background jobs for sync).
        /// </summary>
        Task<CartCacheDto?> GetCartByKeyAsync(string redisKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Extract the CustomerId from a Redis cart key.
        /// </summary>
        CustomerId? ExtractCustomerIdFromKey(string redisKey);

        /// <summary>
        /// Move cart from Redis to Database at checkout.
        /// Removes from Redis after successful database persist.
        /// </summary>
        Task<bool> MoveCartToDatabaseAsync(CustomerId customerId, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Lightweight DTO for cart data stored in Redis.
    /// Not a domain entity - this is a cache representation.
    /// </summary>
    public sealed class CartCacheDto
    {
        public Guid CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public List<CartItemCacheDto> Items { get; set; } = new();
        public int TotalItems => Items.Sum(i => i.Quantity);
    }

    /// <summary>
    /// Lightweight DTO for cart item data stored in Redis.
    /// </summary>
    public sealed class CartItemCacheDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
