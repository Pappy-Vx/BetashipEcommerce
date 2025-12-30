using BetashipEcommerce.CORE.Products.Entities;
using BetashipEcommerce.CORE.Products.Enums;
using BetashipEcommerce.CORE.Products.Events;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Products
{
    public sealed class Product : AggregateRoot<ProductId>
    {
        private readonly List<ProductImage> _images = new();
        private readonly List<ProductVariant> _variants = new();

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Sku { get; private set; }
        public Money Price { get; private set; }
        public ProductCategory Category { get; private set; }
        public int StockQuantity { get; private set; }
        public ProductStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
        public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

        private Product(
            ProductId id,
            string name,
            string description,
            string sku,
            Money price,
            ProductCategory category,
            int stockQuantity) : base(id)
        {
            Name = name;
            Description = description;
            Sku = sku;
            Price = price;
            Category = category;
            StockQuantity = stockQuantity;
            Status = ProductStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }

        private Product() : base() { } // For EF Core

        public static Result<Product> Create(
            string name,
            string description,
            string sku,
            Money price,
            ProductCategory category,
            int initialStock)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<Product>(ProductErrors.InvalidName);

            if (string.IsNullOrWhiteSpace(sku))
                return Result.Failure<Product>(ProductErrors.InvalidSku);

            if (price.Amount <= 0)
                return Result.Failure<Product>(ProductErrors.InvalidPrice);

            if (initialStock < 0)
                return Result.Failure<Product>(ProductErrors.InvalidStock);

            var product = new Product(
                new ProductId(Guid.NewGuid()),
                name,
                description,
                sku,
                price,
                category,
                initialStock);

            product.RaiseDomainEvent(new ProductCreatedDomainEvent(product.Id, product.Name));

            return Result.Success(product);
        }

        public Result UpdatePrice(Money newPrice)
        {
            if (newPrice.Amount <= 0)
                return Result.Failure(ProductErrors.InvalidPrice);

            var oldPrice = Price;
            Price = newPrice;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new ProductPriceChangedDomainEvent(
                Id,
                oldPrice.Amount,
                newPrice.Amount,
                newPrice.Currency));

            return Result.Success();
        }

        public Result AdjustStock(int quantity, string reason)
        {
            if (StockQuantity + quantity < 0)
                return Result.Failure(ProductErrors.InsufficientStock);

            var oldQuantity = StockQuantity;
            StockQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new ProductStockAdjustedDomainEvent(
                Id,
                oldQuantity,
                StockQuantity,
                reason));

            return Result.Success();
        }

        public Result Publish()
        {
            if (Status == ProductStatus.Published)
                return Result.Failure(ProductErrors.AlreadyPublished);

            if (StockQuantity <= 0)
                return Result.Failure(ProductErrors.CannotPublishWithoutStock);

            Status = ProductStatus.Published;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new ProductPublishedDomainEvent(Id, Name));

            return Result.Success();
        }

        public void AddImage(ProductImage image)
        {
            _images.Add(image);
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddVariant(ProductVariant variant)
        {
            _variants.Add(variant);
            UpdatedAt = DateTime.UtcNow;
        }
    }

}
