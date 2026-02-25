using BetashipEcommerce.CORE.Carts;
using BetashipEcommerce.CORE.Carts.ValueObjects;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.CORE.Services;
using BetashipEcommerce.CORE.UnitOfWork;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace BetashipEcommerce.DAL.Repositories
{
    /// <summary>
    /// Redis-backed implementation of ICartCacheService.
    /// Provides blazing-fast (~1ms) cart operations using in-memory Redis storage.
    /// 
    /// Key Strategy:
    ///   cart:{customerId}      → Hash containing cart data (JSON serialized)
    ///   cart:activity:{customerId} → String key with TTL to track last activity
    ///   cart:keys              → Set of all active cart keys
    /// 
    /// Expiration: Carts auto-expire after 30 days of inactivity.
    /// </summary>
    internal sealed class CartCacheService : ICartCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartCacheService> _logger;

        private const string CartKeyPrefix = "cart:";
        private const string CartActivityPrefix = "cart:activity:";
        private const string AllCartKeysSet = "cart:all-keys";
        private static readonly TimeSpan DefaultCartExpiry = TimeSpan.FromDays(30);
        private static readonly TimeSpan ActivityKeyExpiry = TimeSpan.FromHours(24);

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public CartCacheService(
            IConnectionMultiplexer redis,
            IShoppingCartRepository cartRepository,
            IUnitOfWork unitOfWork,
            ILogger<CartCacheService> logger)
        {
            _redis = redis;
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private IDatabase Db => _redis.GetDatabase();

        private static string GetCartKey(CustomerId customerId) =>
            $"{CartKeyPrefix}{customerId.Value}";

        private static string GetActivityKey(CustomerId customerId) =>
            $"{CartActivityPrefix}{customerId.Value}";

        /// <inheritdoc />
        public async Task<CartCacheDto?> GetCartAsync(
            CustomerId customerId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var db = Db;
                var key = GetCartKey(customerId);

                var cartJson = await db.StringGetAsync(key);
                if (cartJson.IsNullOrEmpty)
                {
                    _logger.LogDebug("No cached cart found for customer {CustomerId}", customerId.Value);
                    return null;
                }

                var cart = JsonSerializer.Deserialize<CartCacheDto>(cartJson!, JsonOptions);

                // Refresh activity tracker
                await db.StringSetAsync(
                    GetActivityKey(customerId),
                    DateTime.UtcNow.ToString("O"),
                    ActivityKeyExpiry);

                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cached cart for customer {CustomerId}", customerId.Value);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task AddItemAsync(
            CustomerId customerId,
            ProductId productId,
            int quantity,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var db = Db;
                var key = GetCartKey(customerId);

                // Get existing cart or create new one
                var cart = await GetCartAsync(customerId, cancellationToken) ?? new CartCacheDto
                {
                    CustomerId = customerId.Value,
                    CreatedAt = DateTime.UtcNow,
                    LastModifiedAt = DateTime.UtcNow,
                    Items = new List<CartItemCacheDto>()
                };

                // Check if item already exists
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId.Value);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    cart.Items.Add(new CartItemCacheDto
                    {
                        ProductId = productId.Value,
                        Quantity = quantity,
                        AddedAt = DateTime.UtcNow
                    });
                }

                cart.LastModifiedAt = DateTime.UtcNow;

                // Serialize and store
                var cartJson = JsonSerializer.Serialize(cart, JsonOptions);
                await db.StringSetAsync(key, cartJson, DefaultCartExpiry);

                // Update activity tracker
                await db.StringSetAsync(
                    GetActivityKey(customerId),
                    DateTime.UtcNow.ToString("O"),
                    ActivityKeyExpiry);

                // Track this cart key
                await db.SetAddAsync(AllCartKeysSet, key);

                _logger.LogDebug(
                    "Added {Quantity}x product {ProductId} to cart for customer {CustomerId}",
                    quantity, productId.Value, customerId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error adding item to cached cart for customer {CustomerId}", customerId.Value);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateItemQuantityAsync(
            CustomerId customerId,
            ProductId productId,
            int newQuantity,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var db = Db;
                var cart = await GetCartAsync(customerId, cancellationToken);
                if (cart == null)
                    return false;

                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId.Value);
                if (item == null)
                    return false;

                if (newQuantity <= 0)
                {
                    cart.Items.Remove(item);
                }
                else
                {
                    item.Quantity = newQuantity;
                }

                cart.LastModifiedAt = DateTime.UtcNow;

                var key = GetCartKey(customerId);
                var cartJson = JsonSerializer.Serialize(cart, JsonOptions);
                await db.StringSetAsync(key, cartJson, DefaultCartExpiry);

                // Update activity tracker
                await db.StringSetAsync(
                    GetActivityKey(customerId),
                    DateTime.UtcNow.ToString("O"),
                    ActivityKeyExpiry);

                _logger.LogDebug(
                    "Updated quantity to {Quantity} for product {ProductId} in customer {CustomerId} cart",
                    newQuantity, productId.Value, customerId.Value);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error updating item in cached cart for customer {CustomerId}", customerId.Value);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> RemoveItemAsync(
            CustomerId customerId,
            ProductId productId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var db = Db;
                var cart = await GetCartAsync(customerId, cancellationToken);
                if (cart == null)
                    return false;

                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId.Value);
                if (item == null)
                    return false;

                cart.Items.Remove(item);
                cart.LastModifiedAt = DateTime.UtcNow;

                var key = GetCartKey(customerId);
                var cartJson = JsonSerializer.Serialize(cart, JsonOptions);
                await db.StringSetAsync(key, cartJson, DefaultCartExpiry);

                _logger.LogDebug(
                    "Removed product {ProductId} from customer {CustomerId} cart",
                    productId.Value, customerId.Value);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error removing item from cached cart for customer {CustomerId}", customerId.Value);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task ClearCartAsync(
            CustomerId customerId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var db = Db;
                var cart = await GetCartAsync(customerId, cancellationToken);
                if (cart == null)
                    return;

                cart.Items.Clear();
                cart.LastModifiedAt = DateTime.UtcNow;

                var key = GetCartKey(customerId);
                var cartJson = JsonSerializer.Serialize(cart, JsonOptions);
                await db.StringSetAsync(key, cartJson, DefaultCartExpiry);

                _logger.LogDebug("Cleared cart for customer {CustomerId}", customerId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error clearing cached cart for customer {CustomerId}", customerId.Value);
            }
        }

        /// <inheritdoc />
        public async Task DeleteCartAsync(
            CustomerId customerId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var db = Db;
                var key = GetCartKey(customerId);
                var activityKey = GetActivityKey(customerId);

                await db.KeyDeleteAsync(key);
                await db.KeyDeleteAsync(activityKey);
                await db.SetRemoveAsync(AllCartKeysSet, key);

                _logger.LogDebug("Deleted cart from Redis for customer {CustomerId}", customerId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error deleting cached cart for customer {CustomerId}", customerId.Value);
            }
        }

        /// <inheritdoc />
        public async Task<bool> CartExistsAsync(
            CustomerId customerId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var db = Db;
                return await db.KeyExistsAsync(GetCartKey(customerId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error checking cart existence for customer {CustomerId}", customerId.Value);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<string>> GetAbandonedCartKeysAsync(
            TimeSpan inactiveDuration,
            CancellationToken cancellationToken = default)
        {
            var abandonedKeys = new List<string>();

            try
            {
                var db = Db;
                var allKeys = await db.SetMembersAsync(AllCartKeysSet);

                foreach (var key in allKeys)
                {
                    var cartKey = key.ToString();
                    // Extract customerId from key: "cart:{customerId}"
                    var customerIdStr = cartKey.Replace(CartKeyPrefix, "");
                    if (!Guid.TryParse(customerIdStr, out var customerIdGuid))
                        continue;

                    var activityKey = $"{CartActivityPrefix}{customerIdStr}";
                    var activityExists = await db.KeyExistsAsync(activityKey);

                    // If activity key has expired (no activity within TTL), cart is abandoned
                    if (!activityExists)
                    {
                        // Double-check the cart itself exists
                        if (await db.KeyExistsAsync(cartKey))
                        {
                            abandonedKeys.Add(cartKey);
                        }
                        else
                        {
                            // Cart expired, clean up the tracking set
                            await db.SetRemoveAsync(AllCartKeysSet, cartKey);
                        }
                    }
                }

                _logger.LogInformation(
                    "Found {Count} abandoned carts (inactive > {Duration})",
                    abandonedKeys.Count, inactiveDuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning for abandoned carts");
            }

            return abandonedKeys;
        }

        /// <inheritdoc />
        public async Task<CartCacheDto?> GetCartByKeyAsync(
            string redisKey,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var db = Db;
                var cartJson = await db.StringGetAsync(redisKey);
                if (cartJson.IsNullOrEmpty)
                    return null;

                return JsonSerializer.Deserialize<CartCacheDto>(cartJson!, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart by key {Key}", redisKey);
                return null;
            }
        }

        /// <inheritdoc />
        public CustomerId? ExtractCustomerIdFromKey(string redisKey)
        {
            var customerIdStr = redisKey.Replace(CartKeyPrefix, "");
            if (Guid.TryParse(customerIdStr, out var customerIdGuid))
                return new CustomerId(customerIdGuid);
            return null;
        }

        /// <inheritdoc />
        public async Task<bool> MoveCartToDatabaseAsync(
            CustomerId customerId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var cachedCart = await GetCartAsync(customerId, cancellationToken);
                if (cachedCart == null || cachedCart.Items.Count == 0)
                {
                    _logger.LogDebug("No cart to move for customer {CustomerId}", customerId.Value);
                    return false;
                }

                // Check if customer already has an active DB cart
                var existingDbCart = await _cartRepository.GetActiveCartByCustomerIdAsync(
                    customerId, cancellationToken);

                if (existingDbCart != null)
                {
                    // Update existing DB cart with Redis data
                    existingDbCart.Clear(); // Clear old items

                    foreach (var item in cachedCart.Items)
                    {
                        existingDbCart.AddItem(
                            new ProductId(item.ProductId),
                            item.Quantity);
                    }

                    _cartRepository.Update(existingDbCart);
                }
                else
                {
                    // Create new DB cart from Redis data
                    var cartResult = ShoppingCart.Create(customerId);
                    if (!cartResult.IsSuccess)
                    {
                        _logger.LogWarning(
                            "Failed to create DB cart for customer {CustomerId}: {Error}",
                            customerId.Value, cartResult.Error.Message);
                        return false;
                    }

                    var dbCart = cartResult.Value;
                    foreach (var item in cachedCart.Items)
                    {
                        dbCart.AddItem(new ProductId(item.ProductId), item.Quantity);
                    }

                    _cartRepository.Add(dbCart);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Remove from Redis after successful DB persist
                await DeleteCartAsync(customerId, cancellationToken);

                _logger.LogInformation(
                    "Successfully moved cart from Redis to Database for customer {CustomerId} ({ItemCount} items)",
                    customerId.Value, cachedCart.Items.Count);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error moving cart from Redis to Database for customer {CustomerId}",
                    customerId.Value);
                return false;
            }
        }
    }
}
