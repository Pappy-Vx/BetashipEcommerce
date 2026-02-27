using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.BackgroundJobs
{
    public static class BackgroundJobsSetup
    {
        public static void ConfigureRecurringJobs(this IServiceProvider serviceProvider)
        {
            try
            {
                var recurringJobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();
                var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(BackgroundJobsSetup));

                logger.LogInformation("Configuring recurring jobs...");

                // Process outbox messages every 30 seconds
                recurringJobManager.AddOrUpdate<OutboxMessageProcessorJob>(
                    "process-outbox-messages",
                    job => job.ProcessAsync(CancellationToken.None),
                    "*/30 * * * * *", // Every 30 seconds
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Utc,
                        QueueName = "outbox"
                    });

                // Clean up old outbox messages daily at 2 AM
                recurringJobManager.AddOrUpdate<OutboxCleanupJob>(
                    "cleanup-outbox-messages",
                    job => job.CleanupAsync(30, CancellationToken.None),
                    Cron.Daily(2),
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Utc,
                        QueueName = "maintenance"
                    });

                // Clean up expired cart reservations every 5 minutes
                recurringJobManager.AddOrUpdate<ExpiredReservationsCleanupJob>(
                    "cleanup-expired-reservations",
                    job => job.CleanupAsync(CancellationToken.None),
                    "0 */5 * * * *", // Every 5 minutes (6-field cron: second minute hour day month dayOfWeek)
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Utc,
                        QueueName = "maintenance"
                    });

                // Process abandoned carts daily at 10 AM
                recurringJobManager.AddOrUpdate<AbandonedCartNotificationJob>(
                    "process-abandoned-carts",
                    job => job.ProcessAsync(CancellationToken.None),
                    Cron.Daily(10),
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Utc,
                        QueueName = "notifications"
                    });

                // Check and reorder low stock items daily at 8 AM
                recurringJobManager.AddOrUpdate<LowStockCheckJob>(
                    "check-low-stock",
                    job => job.CheckAsync(CancellationToken.None),
                    Cron.Daily(8),
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Utc,
                        QueueName = "default"
                    });

                // Retry failed payments every hour
                recurringJobManager.AddOrUpdate<FailedPaymentRetryJob>(
                    "retry-failed-payments",
                    job => job.RetryAsync(CancellationToken.None),
                    Cron.Hourly,
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Utc,
                        QueueName = "default"
                    });

                // Sync abandoned carts from Redis to Database every 6 hours
                recurringJobManager.AddOrUpdate<CartSyncBackgroundJob>(
                    "sync-abandoned-carts",
                    job => job.SyncAbandonedCartsAsync(CancellationToken.None),
                    "0 0 */6 * * *", // Every 6 hours (6-field cron: second minute hour day month dayOfWeek)
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Utc,
                        QueueName = "maintenance"
                    });

                // Cart cache health check every hour
                recurringJobManager.AddOrUpdate<CartSyncBackgroundJob>(
                    "cart-cache-health-check",
                    job => job.CartCacheHealthCheckAsync(CancellationToken.None),
                    Cron.Hourly,
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Utc,
                        QueueName = "maintenance"
                    });

                logger.LogInformation("✅ Recurring jobs configured successfully");
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(BackgroundJobsSetup));
                logger.LogError(ex, " Error configuring recurring jobs");
                // Don't rethrow - let the app start even if job configuration fails
            }
        }
    }
}
