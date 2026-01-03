using BetashipEcommerce.CORE.Repositories;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.BackgroundJobs
{
    public sealed class ExpiredReservationsCleanupJob
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<ExpiredReservationsCleanupJob> _logger;

        public ExpiredReservationsCleanupJob(
            IInventoryRepository inventoryRepository,
            ILogger<ExpiredReservationsCleanupJob> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task CleanupAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting expired reservations cleanup");

            var allInventoryItems = await _inventoryRepository.GetAllAsync(cancellationToken);

            var totalCleaned = 0;

            foreach (var item in allInventoryItems)
            {
                var expiredCount = item.Reservations.Count(r => r.IsExpired);
                if (expiredCount > 0)
                {
                    item.CleanExpiredReservations();
                    _inventoryRepository.Update(item);
                    totalCleaned += expiredCount;
                }
            }

            _logger.LogInformation(
                "Expired reservations cleanup completed. Cleaned {Count} reservations",
                totalCleaned);
        }
    }
}
