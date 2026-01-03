using BetashipEcommerce.DAL.Data;
using BetashipEcommerce.DAL.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Repositories
{
    internal sealed class OutboxMessageRepository : IOutboxMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public OutboxMessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(
            int batchSize,
            CancellationToken cancellationToken = default)
        {
            return await _context.OutboxMessages
                .Where(m => m.ProcessedOnUtc == null && m.RetryCount < 5)
                .OrderBy(m => m.OccurredOnUtc)
                .Take(batchSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasBeenProcessedAsync(
            Guid messageId,
            string consumerName,
            CancellationToken cancellationToken = default)
        {
            return await _context.OutboxMessageConsumers
                .AnyAsync(c => c.OutboxMessageId == messageId && c.ConsumerName == consumerName,
                    cancellationToken);
        }

        public async Task MarkAsProcessedAsync(
            Guid messageId,
            string consumerName,
            CancellationToken cancellationToken = default)
        {
            var message = await _context.OutboxMessages
                .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

            if (message != null)
            {
                message.MarkAsProcessed();

                var consumer = OutboxMessageConsumer.Create(messageId, consumerName);
                _context.OutboxMessageConsumers.Add(consumer);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task MarkAsFailedAsync(
            Guid messageId,
            string error,
            CancellationToken cancellationToken = default)
        {
            var message = await _context.OutboxMessages
                .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

            if (message != null)
            {
                message.MarkAsFailed(error);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteProcessedMessagesAsync(
            DateTime olderThan,
            CancellationToken cancellationToken = default)
        {
            var messagesToDelete = await _context.OutboxMessages
                .Where(m => m.ProcessedOnUtc != null && m.ProcessedOnUtc < olderThan)
                .ToListAsync(cancellationToken);

            if (messagesToDelete.Any())
            {
                _context.OutboxMessages.RemoveRange(messagesToDelete);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
