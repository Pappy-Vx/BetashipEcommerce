using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            var recurringJobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();

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
                "*/5 * * * *", // Every 5 minutes
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
        }
    }
}
