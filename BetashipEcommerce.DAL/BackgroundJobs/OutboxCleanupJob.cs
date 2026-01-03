using BetashipEcommerce.DAL.Persistence.Outbox;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.BackgroundJobs
{
    /// <summary>
    /// Cleans up old processed outbox messages
    /// Runs daily at 2 AM
    /// </summary>
    public sealed class OutboxCleanupJob
    {
        private readonly IOutboxMessageRepository _outboxRepository;
        private readonly ILogger<OutboxCleanupJob> _logger;

        public OutboxCleanupJob(
            IOutboxMessageRepository outboxRepository,
            ILogger<OutboxCleanupJob> logger)
        {
            _outboxRepository = outboxRepository;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 2)]
        public async Task CleanupAsync(int retentionDays = 30, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting outbox cleanup job");

            var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

            await _outboxRepository.DeleteProcessedMessagesAsync(cutoffDate, cancellationToken);

            _logger.LogInformation(
                "Completed outbox cleanup. Deleted messages older than {CutoffDate}",
                cutoffDate);
        }
    }
}
