using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Persistence.Outbox
{
    /// <summary>
    /// Outbox message for reliable event publishing
    /// Ensures events are published at-least-once
    /// </summary>
    public sealed class OutboxMessage
    {
        public Guid Id { get; private set; }
        public string Type { get; private set; }
        public string Content { get; private set; }
        public DateTime OccurredOnUtc { get; private set; }
        public DateTime? ProcessedOnUtc { get; private set; }
        public string? Error { get; private set; }
        public int RetryCount { get; private set; }

        private OutboxMessage()
        {
            Id = Guid.Empty;
            Type = string.Empty;
            Content = string.Empty;
            OccurredOnUtc = DateTime.UtcNow;
            RetryCount = 0;
        }

        private OutboxMessage(string type, string content)
        {
            Id = Guid.NewGuid();
            Type = type;
            Content = content;
            OccurredOnUtc = DateTime.UtcNow;
            RetryCount = 0;
        }

        public static OutboxMessage Create(string type, string content)
        {
            return new OutboxMessage(type, content);
        }

        public void MarkAsProcessed()
        {
            ProcessedOnUtc = DateTime.UtcNow;
        }

        public void MarkAsFailed(string error)
        {
            Error = error;
            RetryCount++;
        }
    }
}
