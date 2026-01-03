using BetashipEcommerce.CORE.SharedKernel.Events;
using BetashipEcommerce.DAL.Persistence.Outbox;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.BackgroundJobs
{
    // <summary>
    /// Hangfire job that processes outbox messages and publishes domain events
    /// Runs every 30 seconds
    /// </summary>
    public sealed class OutboxMessageProcessorJob
    {
        private readonly IOutboxMessageRepository _outboxRepository;
        private readonly IPublisher _publisher;
        private readonly ILogger<OutboxMessageProcessorJob> _logger;
        private const int BatchSize = 20;
        private const string ConsumerName = "DomainEventPublisher";

        public OutboxMessageProcessorJob(
            IOutboxMessageRepository outboxRepository,
            IPublisher publisher,
            ILogger<OutboxMessageProcessorJob> logger)
        {
            _outboxRepository = outboxRepository;
            _publisher = publisher;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 30, 60, 120 })]
        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting outbox message processing");

            var messages = await _outboxRepository.GetUnprocessedMessagesAsync(
                BatchSize,
                cancellationToken);

            if (!messages.Any())
            {
                _logger.LogDebug("No unprocessed outbox messages found");
                return;
            }

            _logger.LogInformation("Processing {Count} outbox messages", messages.Count);

            foreach (var message in messages)
            {
                try
                {
                    // Check if already processed by this consumer
                    var hasBeenProcessed = await _outboxRepository.HasBeenProcessedAsync(
                        message.Id,
                        ConsumerName,
                        cancellationToken);

                    if (hasBeenProcessed)
                    {
                        _logger.LogDebug("Message {MessageId} already processed", message.Id);
                        continue;
                    }

                    // Deserialize domain event
                    var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                        message.Content,
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All
                        });

                    if (domainEvent == null)
                    {
                        _logger.LogWarning("Failed to deserialize message {MessageId}", message.Id);
                        await _outboxRepository.MarkAsFailedAsync(
                            message.Id,
                            "Failed to deserialize event",
                            cancellationToken);
                        continue;
                    }

                    // Publish domain event
                    await _publisher.Publish(domainEvent, cancellationToken);

                    // Mark as processed
                    await _outboxRepository.MarkAsProcessedAsync(
                        message.Id,
                        ConsumerName,
                        cancellationToken);

                    _logger.LogInformation(
                        "Successfully processed outbox message {MessageId} of type {Type}",
                        message.Id,
                        message.Type);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error processing outbox message {MessageId}",
                        message.Id);

                    await _outboxRepository.MarkAsFailedAsync(
                        message.Id,
                        ex.Message,
                        cancellationToken);
                }
            }
        }
    }
}
