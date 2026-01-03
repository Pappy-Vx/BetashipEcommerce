using BetashipEcommerce.CORE.Carts;
using BetashipEcommerce.CORE.Carts.ValueObjects;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Repositories
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart, ShoppingCartId>
    {
        Task<ShoppingCart?> GetActiveCartByCustomerIdAsync(
            CustomerId customerId,
            CancellationToken cancellationToken = default);

        Task<List<ShoppingCart>> GetAbandonedCartsAsync(
            DateTime abandonedBefore,
            CancellationToken cancellationToken = default);

        Task<List<ShoppingCart>> GetExpiredCartsAsync(
            CancellationToken cancellationToken = default);
    }
}
