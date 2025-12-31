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

namespace BetashipEcommerce.CORE.Orders.Events
{
    public sealed record OrderCreatedDomainEvent(
      OrderId OrderId,
      CustomerId CustomerId,
      string OrderNumber) : DomainEvent;

    public sealed record OrderConfirmedDomainEvent(
        OrderId OrderId,
        CustomerId CustomerId,
        Money TotalAmount) : DomainEvent;

    public sealed record OrderShippedDomainEvent(
        OrderId OrderId,
        Address ShippingAddress) : DomainEvent;

    public sealed record OrderDeliveredDomainEvent(
        OrderId OrderId) : DomainEvent;

    public sealed record OrderCancelledDomainEvent(
        OrderId OrderId,
        string Reason) : DomainEvent;

    public sealed record OrderInventoryReservedDomainEvent(
    OrderId OrderId,
    List<Guid> ReservationIds) : DomainEvent;

    public sealed record OrderPaymentLinkedDomainEvent(
        OrderId OrderId,
        PaymentId PaymentId) : DomainEvent;

    public sealed record OrderPaidDomainEvent(
        OrderId OrderId,
        CustomerId CustomerId,
        Money Amount,
        DateTime PaidAt) : DomainEvent;

    public sealed record OrderInventoryCommittedDomainEvent(
        OrderId OrderId,
        List<Guid> ReservationIds) : DomainEvent;
}
