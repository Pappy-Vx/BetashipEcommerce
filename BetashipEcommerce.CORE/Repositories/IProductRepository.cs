using BetashipEcommerce.CORE.Products;
using BetashipEcommerce.CORE.Products.Enums;
using BetashipEcommerce.CORE.Products.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Repositories
{
    public interface IProductRepository : IRepository<Product, ProductId>
    {
        Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Product>> GetByIdsAsync(
            IEnumerable<ProductId> ids,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Product>> GetPublishedProductsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Product>> GetByCategoryAsync(
            ProductCategory category,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Product>> SearchAsync(
            string searchTerm,
            CancellationToken cancellationToken = default);

        Task<bool> IsSkuUniqueAsync(
            string sku,
            ProductId? excludeProductId = null,
            CancellationToken cancellationToken = default);
    }
}
