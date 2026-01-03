using BetashipEcommerce.CORE.Inventory;
using BetashipEcommerce.CORE.Inventory.ValueObjects;
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
    internal sealed class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.ToTable("InventoryItems", "ecommerce");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .HasConversion(
                    id => id.Value,
                    value => new InventoryItemId(value))
                .HasColumnName("InventoryItemId");

            builder.Property(i => i.ProductId)
                .HasConversion(
                    id => id.Value,
                    value => new ProductId(value))
                .IsRequired();

            builder.Property(i => i.AvailableQuantity).IsRequired();
            builder.Property(i => i.ReservedQuantity).IsRequired();
            builder.Property(i => i.ReorderLevel).IsRequired();
            builder.Property(i => i.ReorderQuantity).IsRequired();
            builder.Property(i => i.LastRestocked).IsRequired();

            // Stock reservations
            builder.OwnsMany(i => i.Reservations, reservations =>
            {
                reservations.ToTable("StockReservations", "ecommerce");
                reservations.WithOwner().HasForeignKey("InventoryItemId");
                reservations.HasKey("Id");

                reservations.Property(r => r.ProductId)
                    .HasConversion(
                        id => id.Value,
                        value => new ProductId(value))
                    .IsRequired();

                reservations.Property(r => r.Quantity).IsRequired();

                reservations.Property(r => r.ReservedFor)
                    .IsRequired()
                    .HasMaxLength(200);

                reservations.Property(r => r.ReservedAt).IsRequired();
                reservations.Property(r => r.ExpiresAt).IsRequired();

                reservations.Property(r => r.Status)
                    .IsRequired()
                    .HasConversion<int>();

                reservations.HasIndex(r => r.ExpiresAt)
                    .HasDatabaseName("IX_StockReservations_ExpiresAt");

                reservations.HasIndex(r => r.Status)
                    .HasDatabaseName("IX_StockReservations_Status");
            });

            // Indexes
            builder.HasIndex(i => i.ProductId)
                .IsUnique()
                .HasDatabaseName("IX_InventoryItems_ProductId");

            builder.HasIndex(i => i.AvailableQuantity)
                .HasDatabaseName("IX_InventoryItems_AvailableQuantity");

            builder.Ignore(i => i.DomainEvents);
        }
    }

}
