using BetashipEcommerce.CORE.Carts;
using BetashipEcommerce.CORE.Carts.ValueObjects;
using BetashipEcommerce.CORE.Customers.ValueObjects;
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
    internal sealed class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.ToTable("ShoppingCarts", "ecommerce");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasConversion(
                    id => id.Value,
                    value => new ShoppingCartId(value))
                .HasColumnName("ShoppingCartId");

            builder.Property(c => c.CustomerId)
                .HasConversion(
                    id => id.Value,
                    value => new CustomerId(value))
                .IsRequired();

            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.LastModifiedAt).IsRequired();
            builder.Property(c => c.ExpiresAt);

            // Cart items
            builder.OwnsMany(c => c.Items, items =>
            {
                items.ToTable("CartItems", "ecommerce");
                items.WithOwner().HasForeignKey("ShoppingCartId");
                items.HasKey("Id");

                items.Property(i => i.ProductId)
                    .HasConversion(
                        id => id.Value,
                        value => new ProductId(value))
                    .IsRequired();

                items.Property(i => i.Quantity).IsRequired();
                items.Property(i => i.AddedAt).IsRequired();
                items.Property(i => i.LastModifiedAt).IsRequired();

                items.HasIndex(i => i.ProductId)
                    .HasDatabaseName("IX_CartItems_ProductId");
            });

            // Indexes
            builder.HasIndex(c => c.CustomerId)
                .HasDatabaseName("IX_ShoppingCarts_CustomerId");

            builder.HasIndex(c => c.Status)
                .HasDatabaseName("IX_ShoppingCarts_Status");

            builder.HasIndex(c => c.ExpiresAt)
                .HasDatabaseName("IX_ShoppingCarts_ExpiresAt");

            builder.Ignore(c => c.DomainEvents);
        }
    }
}
