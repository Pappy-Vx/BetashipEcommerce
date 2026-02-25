using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Services;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace BetashipEcommerce.DAL.BackgroundJobs
{
    /// <summary>
    /// Background job that syncs abandoned carts from Redis to Database.
    /// 
    /// Strategy:
    /// - Scans Redis for carts that have been inactive for 24+ hours
    /// - Moves abandoned carts to PostgreSQL for analytics and recovery
    /// - Removes synced carts from Redis to free memory
    /// 
    /// This is a critical part of the hybrid (Redis + Database) cart strategy
    /// used by major e-commerce platforms (Amazon, eBay, Shopify).
    /// </summary>
    public sealed class CartSyncBackgroundJob
    {
        private readonly ICartCacheService _cartCacheService;
        private readonly ILogger<CartSyncBackgroundJob> _logger;

        // Carts inactive for more than 24 hours are considered abandoned
        private static readonly TimeSpan AbandonedThreshold = TimeSpan.FromHours(24);

        public CartSyncBackgroundJob(
            ICartCacheService cartCacheService,
            ILogger<CartSyncBackgroundJob> logger)
        {
            _cartCacheService = cartCacheService;
            _logger = logger;
        }

        /// <summary>
        /// Sync abandoned carts from Redis to Database.
        /// Runs every 6 hours via Hangfire recurring job.
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task SyncAbandonedCartsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "🔄 Starting abandoned cart sync (Redis → Database). Threshold: {Threshold}",
                AbandonedThreshold);

            try
            {
                var abandonedKeys = await _cartCacheService.GetAbandonedCartKeysAsync(
                    AbandonedThreshold, cancellationToken);

                if (abandonedKeys.Count == 0)
                {
                    _logger.LogInformation("✅ No abandoned carts to sync");
                    return;
                }

                int synced = 0;
                int failed = 0;

                foreach (var key in abandonedKeys)
                {
                    try
                    {
                        var customerId = _cartCacheService.ExtractCustomerIdFromKey(key);
                        if (customerId == null)
                        {
                            _logger.LogWarning("Could not extract CustomerId from key {Key}", key);
                            failed++;
                            continue;
                        }

                        var success = await _cartCacheService.MoveCartToDatabaseAsync(
                            customerId, cancellationToken);

                        if (success)
                        {
                            synced++;
                            _logger.LogDebug(
                                "Synced abandoned cart for customer {CustomerId} to database",
                                customerId.Value);
                        }
                        else
                        {
                            failed++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error syncing abandoned cart {Key}", key);
                        failed++;
                    }
                }

                _logger.LogInformation(
                    "🏁 Abandoned cart sync complete. Synced: {Synced}, Failed: {Failed}, Total: {Total}",
                    synced, failed, abandonedKeys.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Fatal error during abandoned cart sync");
                throw; // Let Hangfire retry
            }
        }

        /// <summary>
        /// Health check - verify Redis connection and report cart stats.
        /// Runs every hour.
        /// </summary>
        [AutomaticRetry(Attempts = 1)]
        public async Task CartCacheHealthCheckAsync(CancellationToken cancellationToken)
        {
            try
            {
                var abandonedKeys = await _cartCacheService.GetAbandonedCartKeysAsync(
                    AbandonedThreshold, cancellationToken);

                _logger.LogInformation(
                    "📊 Cart Cache Health: {AbandonedCount} abandoned carts pending sync",
                    abandonedKeys.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Cart cache health check failed - Redis may be down!");
            }
        }
    }
}
