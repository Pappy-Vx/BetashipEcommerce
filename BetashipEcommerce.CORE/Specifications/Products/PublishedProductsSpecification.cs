using BetashipEcommerce.CORE.Products;
using BetashipEcommerce.CORE.Products.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Specifications.Products
{
    public sealed class PublishedProductsSpecification : Specification<Product>
    {
        public PublishedProductsSpecification(int pageNumber, int pageSize)
            : base(p => p.Status == ProductStatus.Published)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
            ApplyOrderByDescending(p => p.CreatedAt);
            AddInclude(p => p.Images);
        }
    }
}
