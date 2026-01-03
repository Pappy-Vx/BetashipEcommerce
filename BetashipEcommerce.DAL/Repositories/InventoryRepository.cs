using BetashipEcommerce.CORE.Inventory;
using BetashipEcommerce.CORE.Inventory.ValueObjects;
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
    internal sealed class InventoryRepository : Repository<InventoryItem, InventoryItemId>, IInventoryRepository
    {
        public InventoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<InventoryItem?> GetByProductIdAsync(
            ProductId productId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(i => i.Reservations)
                .FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);
        }

        public async Task<List<InventoryItem>> GetLowStockItemsAsync(
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(i => i.AvailableQuantity <= i.ReorderLevel)
                .Include(i => i.Reservations)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<InventoryItem>> GetByProductIdsAsync(
            List<ProductId> productIds,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(i => productIds.Contains(i.ProductId))
                .Include(i => i.Reservations)
                .ToListAsync(cancellationToken);
        }
    }
}
