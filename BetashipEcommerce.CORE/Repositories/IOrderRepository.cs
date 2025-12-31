using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Orders;
using BetashipEcommerce.CORE.Orders.Enums;
using BetashipEcommerce.CORE.Orders.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Repositories
{
    public interface IOrderRepository : IRepository<Order, OrderId>
    {
        Task<Order?> GetByOrderNumberAsync(
            string orderNumber,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Order>> GetByCustomerIdAsync(
            CustomerId customerId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Order>> GetByStatusAsync(
            OrderStatus status,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Order>> GetOrdersForDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Order>> GetPendingOrdersAsync(
            CancellationToken cancellationToken = default);
    }

}
