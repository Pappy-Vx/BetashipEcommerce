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
    internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages", "ecommerce");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Content)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(x => x.OccurredOnUtc)
                .IsRequired();

            builder.Property(x => x.ProcessedOnUtc)
                .IsRequired(false);

            builder.Property(x => x.Error)
                .HasMaxLength(2000);

            builder.Property(x => x.RetryCount)
                .IsRequired();

            // Indexes for performance
            builder.HasIndex(x => x.ProcessedOnUtc)
                .HasFilter("\"ProcessedOnUtc\" IS NULL")
                .HasDatabaseName("IX_OutboxMessages_Unprocessed");

            builder.HasIndex(x => x.OccurredOnUtc)
                .HasDatabaseName("IX_OutboxMessages_OccurredOnUtc");
        }
    }

}
