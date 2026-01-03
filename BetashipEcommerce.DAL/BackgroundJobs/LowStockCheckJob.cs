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
    public sealed class LowStockCheckJob
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<LowStockCheckJob> _logger;

        public LowStockCheckJob(
            IInventoryRepository inventoryRepository,
            ILogger<LowStockCheckJob> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 2)]
        public async Task CheckAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting low stock check");

            var lowStockItems = await _inventoryRepository.GetLowStockItemsAsync(cancellationToken);

            if (!lowStockItems.Any())
            {
                _logger.LogInformation("No low stock items found");
                return;
            }

            _logger.LogWarning(
                "Found {Count} items with low stock",
                lowStockItems.Count);

            foreach (var item in lowStockItems)
            {
                _logger.LogWarning(
                    "Low stock alert: Product {ProductId}, Available: {Available}, Reorder Level: {ReorderLevel}",
                    item.ProductId,
                    item.AvailableQuantity,
                    item.ReorderLevel);

                // TODO: Send notification to inventory managers
                // This could trigger a domain event for notification
            }
        }
    }
}
