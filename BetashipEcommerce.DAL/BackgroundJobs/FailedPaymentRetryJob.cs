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
    public sealed class FailedPaymentRetryJob
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<FailedPaymentRetryJob> _logger;

        public FailedPaymentRetryJob(
            IPaymentRepository paymentRepository,
            ILogger<FailedPaymentRetryJob> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 2)]
        public async Task RetryAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting failed payment retry job");

            var failedPayments = await _paymentRepository.GetFailedPaymentsForRetryAsync(cancellationToken);

            if (!failedPayments.Any())
            {
                _logger.LogDebug("No failed payments to retry");
                return;
            }

            _logger.LogInformation(
                "Found {Count} failed payments to retry",
                failedPayments.Count);

            foreach (var payment in failedPayments)
            {
                try
                {
                    // Reset payment to pending for retry
                    var result = payment.Retry();

                    if (result.IsSuccess)
                    {
                        _paymentRepository.Update(payment);

                        _logger.LogInformation(
                            "Payment {PaymentId} queued for retry (Attempt {Attempt})",
                            payment.Id,
                            payment.RetryCount);

                        // TODO: Trigger payment processing
                        // This would typically enqueue a job to process the payment
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error retrying payment {PaymentId}",
                        payment.Id);
                }
            }
        }
    }
}
