using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Persistence.Outbox
{
    public interface IOutboxMessageRepository
    {
        Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(
            int batchSize,
            CancellationToken cancellationToken = default);

        Task<bool> HasBeenProcessedAsync(
            Guid messageId,
            string consumerName,
            CancellationToken cancellationToken = default);

        Task MarkAsProcessedAsync(
            Guid messageId,
            string consumerName,
            CancellationToken cancellationToken = default);

        Task MarkAsFailedAsync(
            Guid messageId,
            string error,
            CancellationToken cancellationToken = default);

        Task DeleteProcessedMessagesAsync(
            DateTime olderThan,
            CancellationToken cancellationToken = default);
    }

}
