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
    public sealed class Product : AuditableAggregateRoot<ProductId>
    {
        private readonly List<ProductImage> _images = new();
        private readonly List<ProductVariant> _variants = new();

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Sku { get; private set; }
        public Money Price { get; private set; }
        public ProductCategory Category { get; private set; }
        public ProductStatus Status { get; private set; }
        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
        public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

        private Product(
            ProductId id,
            string name,
            string description,
            string sku,
            Money price,
            ProductCategory category) : base(id)
        {
            Name = name;
            Description = description;
            Sku = sku;
            Price = price;
            Category = category;
            Status = ProductStatus.Draft;
        }

        private Product() : base() { }

        public static Result<Product> Create(
            string name,
            string description,
            string sku,
            Money price,
            ProductCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<Product>(ProductErrors.InvalidName);

            if (string.IsNullOrWhiteSpace(sku))
                return Result.Failure<Product>(ProductErrors.InvalidSku);

            if (price.Amount <= 0)
                return Result.Failure<Product>(ProductErrors.InvalidPrice);

            var product = new Product(
                new ProductId(Guid.NewGuid()),
                name,
                description,
                sku,
                price,
                category);

            product.RaiseDomainEvent(new ProductCreatedDomainEvent(product.Id, product.Name));

            return Result.Success(product);
        }

        public Result UpdatePrice(Money newPrice)
        {
            if (newPrice.Amount <= 0)
                return Result.Failure(ProductErrors.InvalidPrice);

            var oldPrice = Price;
            Price = newPrice;

            RaiseDomainEvent(new ProductPriceChangedDomainEvent(
                Id,
                oldPrice.Amount,
                newPrice.Amount,
                newPrice.Currency));

            return Result.Success();
        }

        public Result Publish()
        {
            if (Status == ProductStatus.Published)
                return Result.Failure(ProductErrors.AlreadyPublished);

            Status = ProductStatus.Published;
            RaiseDomainEvent(new ProductPublishedDomainEvent(Id, Name));

            return Result.Success();
        }

        public Result Discontinue(string reason)
        {
            if (Status == ProductStatus.Discontinued)
                return Result.Failure(ProductErrors.AlreadyDiscontinued);

            Status = ProductStatus.Discontinued;
            RaiseDomainEvent(new ProductDiscontinuedDomainEvent(Id, Name, reason));

            return Result.Success();
        }

        public Result UpdateDetails(string name, string description, string sku)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure(ProductErrors.InvalidName);

            if (string.IsNullOrWhiteSpace(sku))
                return Result.Failure(ProductErrors.InvalidSku);

            Name = name;
            Description = description;
            Sku = sku;

            return Result.Success();
        }

        public void AddImage(ProductImage image)
        {
            _images.Add(image);
        }

        public void AddVariant(ProductVariant variant)
        {
            _variants.Add(variant);
        }
    }
}
