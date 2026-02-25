using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.Payments;
using BetashipEcommerce.CORE.Payments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Data.Configurations
{
    internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments", "ecommerce");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasConversion(
                    id => id.Value,
                    value => new PaymentId(value))
                .HasColumnName("PaymentId");

            builder.Property(p => p.OrderId)
                .HasConversion(
                    id => id.Value,
                    value => new OrderId(value))
                .IsRequired();

            builder.Property(p => p.CustomerId)
                .HasConversion(
                    id => id.Value,
                    value => new CustomerId(value))
                .IsRequired();

            builder.OwnsOne(p => p.Amount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Amount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.TransactionId)
                .HasMaxLength(200);

            builder.Property(p => p.PaymentGatewayResponse)
                .HasColumnType("text");

            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.ProcessedAt);
            builder.Property(p => p.CompletedAt);

            builder.Property(p => p.FailureReason)
                .HasMaxLength(500);

            builder.Property(p => p.RetryCount).IsRequired();

            // Indexes
            builder.HasIndex(p => p.OrderId)
                .HasDatabaseName("IX_Payments_OrderId");

            builder.HasIndex(p => p.CustomerId)
                .HasDatabaseName("IX_Payments_CustomerId");

            builder.HasIndex(p => p.TransactionId)
                .HasDatabaseName("IX_Payments_TransactionId");

            builder.HasIndex(p => p.Status)
                .HasDatabaseName("IX_Payments_Status");

            builder.Ignore(p => p.DomainEvents);
        }
    }
}
