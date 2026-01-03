using BetashipEcommerce.CORE.Carts;
using BetashipEcommerce.CORE.Carts.Enum;
using BetashipEcommerce.CORE.Carts.ValueObjects;
using BetashipEcommerce.CORE.Customers.ValueObjects;
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
    internal sealed class ShoppingCartRepository : Repository<ShoppingCart, ShoppingCartId>, IShoppingCartRepository
    {
        public ShoppingCartRepository(ApplicationDbContext context) : base(context) { }

        public async Task<ShoppingCart?> GetActiveCartByCustomerIdAsync(
            CustomerId customerId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(c => c.Items)
                .FirstOrDefaultAsync(
                    c => c.CustomerId == customerId && c.Status == CartStatus.Active,
                    cancellationToken);
        }

        public async Task<List<ShoppingCart>> GetAbandonedCartsAsync(
            DateTime abandonedBefore,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(c => c.Status == CartStatus.Active &&
                           c.LastModifiedAt < abandonedBefore &&
                           c.Items.Any())
                .Include(c => c.Items)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ShoppingCart>> GetExpiredCartsAsync(
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            return await DbSet
                .Where(c => c.ExpiresAt != null && c.ExpiresAt < now)
                .Include(c => c.Items)
                .ToListAsync(cancellationToken);
        }
    }
}
