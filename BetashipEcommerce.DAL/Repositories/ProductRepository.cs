using BetashipEcommerce.CORE.Products;
using BetashipEcommerce.CORE.Products.Enums;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Repositories
{
    internal sealed class ProductRepository : Repository<Product, ProductId>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Product?> GetBySkuAsync(
            string sku,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Sku == sku, cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetByIdsAsync(
            IEnumerable<ProductId> ids,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(p => ids.Contains(p.Id))
                .Include(p => p.Images)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetPublishedProductsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(p => p.Status == ProductStatus.Published)
                .Include(p => p.Images)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetByCategoryAsync(
            ProductCategory category,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(p => p.Category == category && p.Status == ProductStatus.Published)
                .Include(p => p.Images)
                .OrderBy(p => p.Name)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> SearchAsync(
            string searchTerm,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(p => p.Status == ProductStatus.Published &&
                           (EF.Functions.Like(p.Name, $"%{searchTerm}%") ||
                            EF.Functions.Like(p.Description, $"%{searchTerm}%")))
                .Include(p => p.Images)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsSkuUniqueAsync(
            string sku,
            ProductId? excludeProductId = null,
            CancellationToken cancellationToken = default)
        {
            var query = DbSet.Where(p => p.Sku == sku);

            if (excludeProductId != null)
            {
                query = query.Where(p => p.Id != excludeProductId);
            }

            return !await query.AnyAsync(cancellationToken);
        }
    }
}
