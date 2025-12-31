using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.Payments.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Payments.Events
{
    public sealed record PaymentInitiatedDomainEvent(
    PaymentId PaymentId,
    OrderId OrderId,
    CustomerId CustomerId,
    Money Amount) : DomainEvent;

    public sealed record PaymentProcessingDomainEvent(
        PaymentId PaymentId,
        OrderId OrderId) : DomainEvent;

    public sealed record PaymentCompletedDomainEvent(
        PaymentId PaymentId,
        OrderId OrderId,
        CustomerId CustomerId,
        Money Amount,
        string TransactionId) : DomainEvent;

    public sealed record PaymentFailedDomainEvent(
        PaymentId PaymentId,
        OrderId OrderId,
        CustomerId CustomerId,
        string Reason) : DomainEvent;

    public sealed record PaymentRetryingDomainEvent(
        PaymentId PaymentId,
        OrderId OrderId,
        int RetryCount) : DomainEvent;

    public sealed record PaymentRefundedDomainEvent(
        PaymentId PaymentId,
        OrderId OrderId,
        Money RefundAmount,
        string Reason) : DomainEvent;

    public sealed record PaymentCancelledDomainEvent(
        PaymentId PaymentId,
        OrderId OrderId,
        string Reason) : DomainEvent;
}
