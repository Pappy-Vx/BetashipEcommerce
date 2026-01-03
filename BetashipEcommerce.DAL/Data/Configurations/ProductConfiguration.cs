using BetashipEcommerce.CORE.Products;
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
    internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", "ecommerce");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasConversion(
                    id => id.Value,
                    value => new ProductId(value))
                .HasColumnName("ProductId");

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasMaxLength(2000);

            builder.Property(p => p.Sku)
                .IsRequired()
                .HasMaxLength(50);

            // Complex type for Money
            builder.OwnsOne(p => p.Price, price =>
            {
                price.Property(m => m.Amount)
                    .HasColumnName("Price")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                price.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.Property(p => p.Category)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<int>();

            // Audit fields
            //builder.Property(p => p.CreatedAt).IsRequired();
            //builder.Property(p => p.CreatedBy);
            //builder.Property(p => p.CreatedByUsername).HasMaxLength(100);
            //builder.Property(p => p.UpdatedAt);
            //builder.Property(p => p.UpdatedBy);
            //builder.Property(p => p.UpdatedByUsername).HasMaxLength(100);
            //builder.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);
            //builder.Property(p => p.DeletedAt);
            //builder.Property(p => p.DeletedBy);
            //builder.Property(p => p.DeletedByUsername).HasMaxLength(100);

            // Owned entities - Images
            builder.OwnsMany(p => p.Images, images =>
            {
                images.ToTable("ProductImages", "ecommerce");
                images.WithOwner().HasForeignKey("ProductId");
                images.HasKey("Id");

                images.Property(i => i.Url)
                    .IsRequired()
                    .HasMaxLength(500);

                images.Property(i => i.AltText)
                    .HasMaxLength(200);

                images.Property(i => i.SortOrder)
                    .IsRequired();

                images.Property(i => i.IsPrimary)
                    .IsRequired();
            });

            // Owned entities - Variants
            builder.OwnsMany(p => p.Variants, variants =>
            {
                variants.ToTable("ProductVariants", "ecommerce");
                variants.WithOwner().HasForeignKey("ProductId");
                variants.HasKey("Id");

                variants.Property(v => v.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                variants.Property(v => v.Sku)
                    .IsRequired()
                    .HasMaxLength(50);

                variants.OwnsOne(v => v.Price, price =>
                {
                    price.Property(m => m.Amount)
                        .HasColumnName("Price")
                        .HasColumnType("decimal(18,2)");

                    price.Property(m => m.Currency)
                        .HasColumnName("Currency")
                        .HasMaxLength(3);
                });

                variants.Property(v => v.StockQuantity)
                    .IsRequired();
            });

            // Indexes
            builder.HasIndex(p => p.Sku)
                .IsUnique()
                .HasDatabaseName("IX_Products_Sku");

            builder.HasIndex(p => p.Status)
                .HasDatabaseName("IX_Products_Status");

            builder.HasIndex(p => p.Category)
                .HasDatabaseName("IX_Products_Category");

            builder.HasIndex(p => p.Name)
                .HasDatabaseName("IX_Products_Name");

            // Ignore domain events (not persisted)
            builder.Ignore(p => p.DomainEvents);
        }
    }

}
