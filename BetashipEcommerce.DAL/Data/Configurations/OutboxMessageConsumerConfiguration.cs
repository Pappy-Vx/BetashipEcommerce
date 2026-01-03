using BetashipEcommerce.DAL.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Data.Configurations
{
    internal sealed class OutboxMessageConsumerConfiguration : IEntityTypeConfiguration<OutboxMessageConsumer>
    {
        public void Configure(EntityTypeBuilder<OutboxMessageConsumer> builder)
        {
            builder.ToTable("OutboxMessageConsumers", "ecommerce");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ConsumerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.ProcessedOnUtc)
                .IsRequired();

            // Unique constraint to prevent duplicate processing
            builder.HasIndex(x => new { x.OutboxMessageId, x.ConsumerName })
                .IsUnique()
                .HasDatabaseName("IX_OutboxMessageConsumers_Unique");
        }
    }

}
