using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.Payments.Enums;
using BetashipEcommerce.CORE.Payments.Events;
using BetashipEcommerce.CORE.Payments.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Payments
{
    public sealed class Payment : AggregateRoot<PaymentId>
    {
        public OrderId OrderId { get; private set; }
        public CustomerId CustomerId { get; private set; }
        public Money Amount { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus Status { get; private set; }
        public string? TransactionId { get; private set; }
        public string? PaymentGatewayResponse { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ProcessedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public string? FailureReason { get; private set; }
        public int RetryCount { get; private set; }

        private Payment(
            PaymentId id,
            OrderId orderId,
            CustomerId customerId,
            Money amount,
            PaymentMethod paymentMethod) : base(id)
        {
            OrderId = orderId;
            CustomerId = customerId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            RetryCount = 0;
        }

        private Payment() : base() { }

        public static Result<Payment> Create(
            OrderId orderId,
            CustomerId customerId,
            Money amount,
            PaymentMethod paymentMethod)
        {
            if (amount.Amount <= 0)
                return Result.Failure<Payment>(PaymentErrors.InvalidAmount);

            var payment = new Payment(
                new PaymentId(Guid.NewGuid()),
                orderId,
                customerId,
                amount,
                paymentMethod);

            payment.RaiseDomainEvent(new PaymentInitiatedDomainEvent(
                payment.Id,
                orderId,
                customerId,
                amount));

            return Result.Success(payment);
        }

        /// <summary>
        /// Mark payment as processing
        /// </summary>
        public Result StartProcessing()
        {
            if (Status != PaymentStatus.Pending)
                return Result.Failure(PaymentErrors.InvalidStatusTransition);

            Status = PaymentStatus.Processing;
            ProcessedAt = DateTime.UtcNow;

            RaiseDomainEvent(new PaymentProcessingDomainEvent(Id, OrderId));

            return Result.Success();
        }

        /// <summary>
        /// Complete payment successfully
        /// </summary>
        public Result Complete(string transactionId, string? gatewayResponse = null)
        {
            if (Status != PaymentStatus.Processing)
                return Result.Failure(PaymentErrors.InvalidStatusTransition);

            if (string.IsNullOrWhiteSpace(transactionId))
                return Result.Failure(PaymentErrors.InvalidTransactionId);

            Status = PaymentStatus.Completed;
            TransactionId = transactionId;
            PaymentGatewayResponse = gatewayResponse;
            CompletedAt = DateTime.UtcNow;

            RaiseDomainEvent(new PaymentCompletedDomainEvent(
                Id,
                OrderId,
                CustomerId,
                Amount,
                transactionId));

            return Result.Success();
        }

        /// <summary>
        /// Mark payment as failed
        /// </summary>
        public Result Fail(string reason, string? gatewayResponse = null)
        {
            if (Status == PaymentStatus.Completed || Status == PaymentStatus.Refunded)
                return Result.Failure(PaymentErrors.CannotFailCompletedPayment);

            Status = PaymentStatus.Failed;
            FailureReason = reason;
            PaymentGatewayResponse = gatewayResponse;
            CompletedAt = DateTime.UtcNow;

            RaiseDomainEvent(new PaymentFailedDomainEvent(
                Id,
                OrderId,
                CustomerId,
                reason));

            return Result.Success();
        }

        /// <summary>
        /// Retry failed payment
        /// </summary>
        public Result Retry()
        {
            if (Status != PaymentStatus.Failed)
                return Result.Failure(PaymentErrors.CanOnlyRetryFailedPayments);

            if (RetryCount >= 3)
                return Result.Failure(PaymentErrors.MaxRetryAttemptsExceeded);

            Status = PaymentStatus.Pending;
            RetryCount++;
            FailureReason = null;

            RaiseDomainEvent(new PaymentRetryingDomainEvent(Id, OrderId, RetryCount));

            return Result.Success();
        }

        /// <summary>
        /// Refund completed payment
        /// </summary>
        public Result Refund(Money refundAmount, string reason)
        {
            if (Status != PaymentStatus.Completed)
                return Result.Failure(PaymentErrors.CanOnlyRefundCompletedPayments);

            if (refundAmount.Amount <= 0 || refundAmount.Amount > Amount.Amount)
                return Result.Failure(PaymentErrors.InvalidRefundAmount);

            if (refundAmount.Currency != Amount.Currency)
                return Result.Failure(PaymentErrors.CurrencyMismatch);

            Status = PaymentStatus.Refunded;

            RaiseDomainEvent(new PaymentRefundedDomainEvent(
                Id,
                OrderId,
                refundAmount,
                reason));

            return Result.Success();
        }

        /// <summary>
        /// Cancel pending payment
        /// </summary>
        public Result Cancel(string reason)
        {
            if (Status == PaymentStatus.Completed || Status == PaymentStatus.Refunded)
                return Result.Failure(PaymentErrors.CannotCancelCompletedPayment);

            Status = PaymentStatus.Cancelled;
            FailureReason = reason;
            CompletedAt = DateTime.UtcNow;

            RaiseDomainEvent(new PaymentCancelledDomainEvent(Id, OrderId, reason));

            return Result.Success();
        }
    }
}
