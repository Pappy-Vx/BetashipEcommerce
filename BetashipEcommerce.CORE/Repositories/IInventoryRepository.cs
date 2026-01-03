using BetashipEcommerce.CORE.Inventory;
using BetashipEcommerce.CORE.Inventory.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Repositories
{
    public interface IInventoryRepository : IRepository<InventoryItem, InventoryItemId>
    {
        Task<InventoryItem?> GetByProductIdAsync(
            ProductId productId,
            CancellationToken cancellationToken = default);

        Task<List<InventoryItem>> GetLowStockItemsAsync(
            CancellationToken cancellationToken = default);

        Task<List<InventoryItem>> GetByProductIdsAsync(
            List<ProductId> productIds,
            CancellationToken cancellationToken = default);
    }
}
