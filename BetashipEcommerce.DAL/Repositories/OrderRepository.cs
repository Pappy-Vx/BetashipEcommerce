using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Orders;
using BetashipEcommerce.CORE.Orders.Enums;
using BetashipEcommerce.CORE.Orders.ValueObjects;
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
    internal sealed class OrderRepository : Repository<Order, OrderId>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Order?> GetByOrderNumberAsync(
            string orderNumber,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(
            CustomerId customerId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.Items)
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetByStatusAsync(
            OrderStatus status,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(o => o.Status == status)
                .Include(o => o.Items)
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Include(o => o.Items)
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetPendingOrdersAsync(
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(o => o.Status == OrderStatus.Pending)
                .Include(o => o.Items)
                .OrderBy(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }
    }

}
