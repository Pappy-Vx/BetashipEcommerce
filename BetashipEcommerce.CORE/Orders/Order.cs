using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Orders.Entities;
using BetashipEcommerce.CORE.Orders.Enums;
using BetashipEcommerce.CORE.Orders.Events;
using BetashipEcommerce.CORE.Orders.ValueObjects;
using BetashipEcommerce.CORE.Payments.ValueObjects;
using BetashipEcommerce.CORE.Products.ValueObjects;
using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Orders
{
    /// <summary>
    /// Enhanced Order aggregate that tracks inventory reservations and payment
    /// Replaces the basic Order aggregate with enterprise features
    /// </summary>
    public sealed class Order : AuditableAggregateRoot<OrderId>
    {
        private readonly List<OrderItem> _items = new();
        private readonly List<Guid> _inventoryReservationIds = new();

        public CustomerId CustomerId { get; private set; }
        public string OrderNumber { get; private set; }
        public OrderStatus Status { get; private set; }
        public Address ShippingAddress { get; private set; }
        public Address BillingAddress { get; private set; }
        public Money TotalAmount { get; private set; }
        public Money SubtotalAmount { get; private set; }
        public Money TaxAmount { get; private set; }
        public Money ShippingAmount { get; private set; }

        // Payment tracking
        public PaymentId? PaymentId { get; private set; }
        public bool IsPaid { get; private set; }
        public DateTime? PaidAt { get; private set; }

        // Inventory tracking
        public bool InventoryReserved { get; private set; }
        public bool InventoryCommitted { get; private set; }
        public IReadOnlyCollection<Guid> InventoryReservationIds => _inventoryReservationIds.AsReadOnly();

        public DateTime OrderDate { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }
        public DateTime? ShippedAt { get; private set; }
        public DateTime? DeliveredAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }
        public string? CancellationReason { get; private set; }

        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private Order(
            OrderId id,
            CustomerId customerId,
            string orderNumber,
            Address shippingAddress,
            Address billingAddress) : base(id)
        {
            CustomerId = customerId;
            OrderNumber = orderNumber;
            Status = OrderStatus.Pending;
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            OrderDate = DateTime.UtcNow;
            SubtotalAmount = Money.Create(0, "NGN");
            TaxAmount = Money.Create(0, "NGN");
            ShippingAmount = Money.Create(0, "NGN");
            TotalAmount = Money.Create(0, "NGN");
            IsPaid = false;
            InventoryReserved = false;
            InventoryCommitted = false;
        }

        private Order() : base() { }

        /// <summary>
        /// Create order with current product prices from cart
        /// Inventory must be reserved separately via IInventoryReservationService
        /// </summary>
        public static Result<Order> Create(
            CustomerId customerId,
            Address shippingAddress,
            Address billingAddress,
            List<(ProductId ProductId, int Quantity, Money UnitPrice, string ProductName)> items)
        {
            if (items == null || items.Count == 0)
                return Result.Failure<Order>(OrderErrors.EmptyOrder);

            var orderId = new OrderId(Guid.NewGuid());
            var orderNumber = GenerateOrderNumber();

            var order = new Order(
                orderId,
                customerId,
                orderNumber,
                shippingAddress,
                billingAddress);

            foreach (var item in items)
            {
                if (item.Quantity <= 0)
                    return Result.Failure<Order>(OrderErrors.InvalidQuantity);

                var orderItem = OrderItem.CreateWithPrice(
                    item.ProductId,
                    item.ProductName,
                    item.Quantity,
                    item.UnitPrice);

                order._items.Add(orderItem);
            }

            order.CalculateTotals();
            order.RaiseDomainEvent(new OrderCreatedDomainEvent(orderId, customerId, orderNumber));

            return Result.Success(order);
        }

        /// <summary>
        /// Associate inventory reservations with this order
        /// </summary>
        public Result LinkInventoryReservations(List<Guid> reservationIds)
        {
            if (Status != OrderStatus.Pending)
                return Result.Failure(OrderErrors.CannotModifyConfirmedOrder);

            if (InventoryReserved)
                return Result.Failure(OrderErrors.InventoryAlreadyReserved);

            _inventoryReservationIds.AddRange(reservationIds);
            InventoryReserved = true;

            RaiseDomainEvent(new OrderInventoryReservedDomainEvent(Id, reservationIds));

            return Result.Success();
        }

        /// <summary>
        /// Associate payment with this order
        /// </summary>
        public Result LinkPayment(PaymentId paymentId)
        {
            if (Status != OrderStatus.Pending)
                return Result.Failure(OrderErrors.CannotModifyConfirmedOrder);

            PaymentId = paymentId;

            RaiseDomainEvent(new OrderPaymentLinkedDomainEvent(Id, paymentId));

            return Result.Success();
        }

        /// <summary>
        /// Mark order as paid (called after payment completion)
        /// </summary>
        public Result MarkAsPaid()
        {
            if (Status != OrderStatus.Pending)
                return Result.Failure(OrderErrors.InvalidStatusTransition);

            if (IsPaid)
                return Result.Failure(OrderErrors.OrderAlreadyPaid);

            if (!InventoryReserved)
                return Result.Failure(OrderErrors.InventoryNotReserved);

            IsPaid = true;
            PaidAt = DateTime.UtcNow;

            RaiseDomainEvent(new OrderPaidDomainEvent(Id, CustomerId, TotalAmount, PaidAt.Value));

            return Result.Success();
        }

        /// <summary>
        /// Mark inventory as committed (after payment completed)
        /// </summary>
        public Result CommitInventory()
        {
            if (!IsPaid)
                return Result.Failure(OrderErrors.OrderNotPaid);

            if (!InventoryReserved)
                return Result.Failure(OrderErrors.InventoryNotReserved);

            if (InventoryCommitted)
                return Result.Failure(OrderErrors.InventoryAlreadyCommitted);

            InventoryCommitted = true;

            RaiseDomainEvent(new OrderInventoryCommittedDomainEvent(Id, _inventoryReservationIds.ToList()));

            return Result.Success();
        }

        /// <summary>
        /// Confirm order (after payment and inventory committed)
        /// </summary>
        public Result Confirm()
        {
            if (Status != OrderStatus.Pending)
                return Result.Failure(OrderErrors.InvalidStatusTransition);

            if (!IsPaid)
                return Result.Failure(OrderErrors.OrderNotPaid);

            if (!InventoryCommitted)
                return Result.Failure(OrderErrors.InventoryNotCommitted);

            if (_items.Count == 0)
                return Result.Failure(OrderErrors.EmptyOrder);

            Status = OrderStatus.Confirmed;
            ConfirmedAt = DateTime.UtcNow;

            RaiseDomainEvent(new OrderConfirmedDomainEvent(Id, CustomerId, TotalAmount));

            return Result.Success();
        }

        /// <summary>
        /// Ship order
        /// </summary>
        public Result Ship(string trackingNumber = null)
        {
            if (Status != OrderStatus.Confirmed)
                return Result.Failure(OrderErrors.InvalidStatusTransition);

            Status = OrderStatus.Shipped;
            ShippedAt = DateTime.UtcNow;

            RaiseDomainEvent(new OrderShippedDomainEvent(Id, ShippingAddress));

            return Result.Success();
        }

        /// <summary>
        /// Mark order as delivered
        /// </summary>
        public Result Deliver()
        {
            if (Status != OrderStatus.Shipped)
                return Result.Failure(OrderErrors.InvalidStatusTransition);

            Status = OrderStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;

            RaiseDomainEvent(new OrderDeliveredDomainEvent(Id));

            return Result.Success();
        }

        /// <summary>
        /// Cancel order (releases inventory if not committed)
        /// </summary>
        public Result Cancel(string reason)
        {
            if (Status == OrderStatus.Delivered)
                return Result.Failure(OrderErrors.CannotCancelDeliveredOrder);

            if (Status == OrderStatus.Cancelled)
                return Result.Failure(OrderErrors.OrderAlreadyCancelled);

            // Can cancel if inventory committed but not shipped
            if (Status == OrderStatus.Shipped)
                return Result.Failure(OrderErrors.CannotCancelShippedOrder);

            Status = OrderStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
            CancellationReason = reason;

            RaiseDomainEvent(new OrderCancelledDomainEvent(
                Id,
                reason
                //InventoryCommitted,
                //_inventoryReservationIds.ToList()
                ));

            return Result.Success();
        }

        private void CalculateTotals()
        {
            SubtotalAmount = _items
                .Select(i => i.TotalPrice)
                .Aggregate(Money.Create(0, "NGN"), (acc, price) => acc.Add(price));

            // Tax calculation (configurable)
            TaxAmount = Money.Create(SubtotalAmount.Amount * 0.10m, "NGN");

            // Shipping calculation (could be more complex)
            ShippingAmount = Money.Create(10.00m, "NGN");

            TotalAmount = SubtotalAmount
                .Add(TaxAmount)
                .Add(ShippingAmount);
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }
    }

}
