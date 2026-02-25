using BetashipEcommerce.CORE.Orders;
using BetashipEcommerce.CORE.Orders.Entities;
using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Data.Configurations
{
    internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", "ecommerce");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .HasConversion(
                    id => id.Value,
                    value => new OrderId(value))
                .HasColumnName("OrderId");

            builder.Property(o => o.CustomerId)
                .HasConversion(
                    id => id.Value,
                    value => new CORE.Customers.ValueObjects.CustomerId(value))
                .IsRequired();

            builder.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<int>();

            // Complex types for addresses
            builder.OwnsOne(o => o.ShippingAddress, address =>
            {
                address.Property(a => a.Street).HasColumnName("ShippingStreet").HasMaxLength(200);
                address.Property(a => a.City).HasColumnName("ShippingCity").HasMaxLength(100);
                address.Property(a => a.State).HasColumnName("ShippingState").HasMaxLength(100);
                address.Property(a => a.Country).HasColumnName("ShippingCountry").HasMaxLength(100);
                address.Property(a => a.PostalCode).HasColumnName("ShippingPostalCode").HasMaxLength(20);
            });

            builder.OwnsOne(o => o.BillingAddress, address =>
            {
                address.Property(a => a.Street).HasColumnName("BillingStreet").HasMaxLength(200);
                address.Property(a => a.City).HasColumnName("BillingCity").HasMaxLength(100);
                address.Property(a => a.State).HasColumnName("BillingState").HasMaxLength(100);
                address.Property(a => a.Country).HasColumnName("BillingCountry").HasMaxLength(100);
                address.Property(a => a.PostalCode).HasColumnName("BillingPostalCode").HasMaxLength(20);
            });

            // Money value objects
            builder.OwnsOne(o => o.TotalAmount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("TotalAmount").HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("TotalCurrency").HasMaxLength(3);
            });

            builder.OwnsOne(o => o.SubtotalAmount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("SubtotalAmount").HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("SubtotalCurrency").HasMaxLength(3);
            });

            builder.OwnsOne(o => o.TaxAmount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("TaxAmount").HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("TaxCurrency").HasMaxLength(3);
            });

            builder.OwnsOne(o => o.ShippingAmount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("ShippingAmount").HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasColumnName("ShippingCurrency").HasMaxLength(3);
            });

            builder.Property(o => o.PaymentId)
                .HasConversion(
                    id => id != null ? id.Value : (Guid?)null,
                    value => value.HasValue ? new CORE.Payments.ValueObjects.PaymentId(value.Value) : null);

            builder.Property(o => o.IsPaid).IsRequired();
            builder.Property(o => o.PaidAt);
            builder.Property(o => o.InventoryReserved).IsRequired();
            builder.Property(o => o.InventoryCommitted).IsRequired();

            // Store inventory reservation IDs as JSON
            builder.Property(o => o.InventoryReservationIds)
                .HasConversion(
                    ids => System.Text.Json.JsonSerializer.Serialize(ids, (System.Text.Json.JsonSerializerOptions)null),
                    json => System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(json, (System.Text.Json.JsonSerializerOptions)null) ?? new List<Guid>())
                .HasColumnName("InventoryReservationIds")
                .HasColumnType("text");

            builder.Property(o => o.OrderDate).IsRequired();
            builder.Property(o => o.ConfirmedAt);
            builder.Property(o => o.ShippedAt);
            builder.Property(o => o.DeliveredAt);
            builder.Property(o => o.CancelledAt);
            builder.Property(o => o.CancellationReason).HasMaxLength(500);

            // Audit fields
            //builder.Property(o => o.CreatedAt).IsRequired();
            //builder.Property(o => o.CreatedBy);
            //builder.Property(o => o.CreatedByUsername).HasMaxLength(100);
            //builder.Property(o => o.UpdatedAt);
            //builder.Property(o => o.UpdatedBy);
            //builder.Property(o => o.UpdatedByUsername).HasMaxLength(100);

            // Order items
            builder.OwnsMany<OrderItem>(o => o.Items, items =>
            //builder.OwnsMany(o => o.Items, items =>
            {
                items.ToTable("OrderItems", "ecommerce");
                items.WithOwner().HasForeignKey("OrderId");
                items.HasKey("Id");

                items.Property(i => i.ProductId)
                    .HasConversion(
                        id => id.Value,
                        value => new ProductId(value))
                    .IsRequired();

                items.Property(i => i.ProductName)
                    .IsRequired()
                    .HasMaxLength(200);

                items.Property(i => i.Quantity)
                    .IsRequired();

                items.OwnsOne(i => i.UnitPrice, price =>
                {
                    price.Property(m => m.Amount).HasColumnName("UnitPrice").HasColumnType("decimal(18,2)");
                    price.Property(m => m.Currency).HasColumnName("UnitPriceCurrency").HasMaxLength(3);
                });

                items.OwnsOne(i => i.TotalPrice, price =>
                {
                    price.Property(m => m.Amount).HasColumnName("TotalPrice").HasColumnType("decimal(18,2)");
                    price.Property(m => m.Currency).HasColumnName("TotalPriceCurrency").HasMaxLength(3);
                });
            });

            // Indexes
            builder.HasIndex(o => o.OrderNumber)
                .IsUnique()
                .HasDatabaseName("IX_Orders_OrderNumber");

            builder.HasIndex(o => o.CustomerId)
                .HasDatabaseName("IX_Orders_CustomerId");

            builder.HasIndex(o => o.Status)
                .HasDatabaseName("IX_Orders_Status");

            builder.HasIndex(o => o.OrderDate)
                .HasDatabaseName("IX_Orders_OrderDate");

            builder.Ignore(o => o.DomainEvents);
        }
    }
}
