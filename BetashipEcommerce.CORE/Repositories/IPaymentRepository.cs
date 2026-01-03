using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.Payments;
using BetashipEcommerce.CORE.Payments.Enums;
using BetashipEcommerce.CORE.Payments.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Repositories
{
    public interface IPaymentRepository : IRepository<Payment, PaymentId>
    {
        Task<Payment?> GetByOrderIdAsync(
            OrderId orderId,
            CancellationToken cancellationToken = default);

        Task<List<Payment>> GetByCustomerIdAsync(
            CustomerId customerId,
            CancellationToken cancellationToken = default);

        Task<Payment?> GetByTransactionIdAsync(
            string transactionId,
            CancellationToken cancellationToken = default);

        Task<List<Payment>> GetFailedPaymentsForRetryAsync(
            CancellationToken cancellationToken = default);

        Task<List<Payment>> GetPendingPaymentsAsync(
            DateTime olderThan,
            CancellationToken cancellationToken = default);
    }
}
