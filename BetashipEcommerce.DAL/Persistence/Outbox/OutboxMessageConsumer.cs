using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Persistence.Outbox
{
    /// <summary>
    /// Tracks which consumers have processed which outbox messages
    /// Enables at-least-once delivery with idempotency
    /// </summary>
    public sealed class OutboxMessageConsumer
    {
        public Guid Id { get; private set; }
        public Guid OutboxMessageId { get; private set; }
        public string ConsumerName { get; private set; }
        public DateTime ProcessedOnUtc { get; private set; }

        private OutboxMessageConsumer()
        {
            Id = Guid.Empty;
            OutboxMessageId = Guid.Empty;
            ConsumerName = string.Empty;
            ProcessedOnUtc = DateTime.UtcNow;
        }

        private OutboxMessageConsumer(Guid outboxMessageId, string consumerName)
        {
            Id = Guid.NewGuid();
            OutboxMessageId = outboxMessageId;
            ConsumerName = consumerName;
            ProcessedOnUtc = DateTime.UtcNow;
        }

        public static OutboxMessageConsumer Create(Guid outboxMessageId, string consumerName)
        {
            return new OutboxMessageConsumer(outboxMessageId, consumerName);
        }
    }
}
