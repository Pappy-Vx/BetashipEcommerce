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
    public sealed class AbandonedCartNotificationJob
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly ILogger<AbandonedCartNotificationJob> _logger;

        public AbandonedCartNotificationJob(
            IShoppingCartRepository cartRepository,
            ILogger<AbandonedCartNotificationJob> logger)
        {
            _cartRepository = cartRepository;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 2)]
        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting abandoned cart processing");

            // Get carts abandoned more than 24 hours ago
            var abandonedBefore = DateTime.UtcNow.AddHours(-24);
            var abandonedCarts = await _cartRepository.GetAbandonedCartsAsync(
                abandonedBefore,
                cancellationToken);

            _logger.LogInformation(
                "Found {Count} abandoned carts",
                abandonedCarts.Count);

            foreach (var cart in abandonedCarts)
            {
                try
                {
                    // Mark cart as abandoned
                    cart.Abandon();
                    _cartRepository.Update(cart);

                    // TODO: Send email notification to customer
                    // This would trigger a domain event that an email handler would process

                    _logger.LogInformation(
                        "Processed abandoned cart {CartId} for customer {CustomerId}",
                        cart.Id,
                        cart.CustomerId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error processing abandoned cart {CartId}",
                        cart.Id);
                }
            }
        }
    }
}
