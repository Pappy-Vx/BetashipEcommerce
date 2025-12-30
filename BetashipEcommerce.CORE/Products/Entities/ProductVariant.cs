using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Products.Entities
{
    public sealed class ProductVariant : Entity<Guid>
    {
        public string Name { get; private set; }
        public string Sku { get; private set; }
        public Money Price { get; private set; }
        public int StockQuantity { get; private set; }

        private ProductVariant(Guid id, string name, string sku, Money price, int stockQuantity)
            : base(id)
        {
            Name = name;
            Sku = sku;
            Price = price;
            StockQuantity = stockQuantity;
        }

        private ProductVariant() : base() { }

        public static ProductVariant Create(string name, string sku, Money price, int stockQuantity)
        {
            return new ProductVariant(Guid.NewGuid(), name, sku, price, stockQuantity);
        }
    }
}
