using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.Payments;
using BetashipEcommerce.CORE.Payments.Enums;
using BetashipEcommerce.CORE.Payments.ValueObjects;
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
    internal sealed class PaymentRepository : Repository<Payment, PaymentId>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Payment?> GetByOrderIdAsync(
            OrderId orderId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);
        }

        public async Task<List<Payment>> GetByCustomerIdAsync(
            CustomerId customerId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Payment?> GetByTransactionIdAsync(
            string transactionId,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId, cancellationToken);
        }

        public async Task<List<Payment>> GetFailedPaymentsForRetryAsync(
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(p => p.Status == PaymentStatus.Failed && p.RetryCount < 3)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Payment>> GetPendingPaymentsAsync(
            DateTime olderThan,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(p => p.Status == PaymentStatus.Pending && p.CreatedAt < olderThan)
                .ToListAsync(cancellationToken);
        }
    }
}
