using BetashipEcommerce.CORE.Products;
using BetashipEcommerce.CORE.Products.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Specifications.Products
{
    public sealed class ProductsByCategorySpecification : Specification<Product>
    {
        public ProductsByCategorySpecification(ProductCategory category)
            : base(p => p.Category == category && p.Status == ProductStatus.Published)
        {
            ApplyOrderBy(p => p.Name);
            AddInclude(p => p.Images);
        }
    }
}
