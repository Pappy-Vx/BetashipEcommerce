using BetashipEcommerce.CORE.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using BetashipEcommerce.DAL.Persistence.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Interceptors
{
    /// <summary>
    /// Converts domain events to outbox messages for reliable event publishing
    /// Implements the Transactional Outbox Pattern
    /// </summary>
    public sealed class DomainEventInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            ConvertDomainEventsToOutboxMessages(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ConvertDomainEventsToOutboxMessages(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void ConvertDomainEventsToOutboxMessages(DbContext? context)
        {
            if (context == null) return;

            var outboxMessages = new List<OutboxMessage>();

            // Get all aggregate roots with domain events
            foreach (var entry in context.ChangeTracker.Entries<AggregateRoot<object>>())
            {
                var domainEvents = entry.Entity.DomainEvents;

                if (!domainEvents.Any())
                    continue;

                // Convert each domain event to outbox message
                foreach (var domainEvent in domainEvents)
                {
                    var outboxMessage = OutboxMessage.Create(
                        domainEvent.GetType().Name,
                        JsonConvert.SerializeObject(domainEvent, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));

                    outboxMessages.Add(outboxMessage);
                }

                // Clear domain events after converting
                entry.Entity.ClearDomainEvents();
            }

            // Add outbox messages to context
            context.Set<OutboxMessage>().AddRange(outboxMessages);
        }
    }

}
