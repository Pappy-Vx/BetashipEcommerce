using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Orders.Entities;
using BetashipEcommerce.CORE.Orders.Enums;
using BetashipEcommerce.CORE.Orders.Events;
using BetashipEcommerce.CORE.Orders.ValueObjects;
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
    public sealed class Order : AggregateRoot<OrderId>
    {
        private readonly List<OrderItem> _items = new();

        public CustomerId CustomerId { get; private set; }
        public string OrderNumber { get; private set; }
        public OrderStatus Status { get; private set; }
        public Address ShippingAddress { get; private set; }
        public Address BillingAddress { get; private set; }
        public Money TotalAmount { get; private set; }
        public Money SubtotalAmount { get; private set; }
        public Money TaxAmount { get; private set; }
        public Money ShippingAmount { get; private set; }
        public DateTime OrderDate { get; private set; }
        public DateTime? CompletedDate { get; private set; }
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
        }

        private Order() : base() { } // For EF Core

        public static Result<Order> Create(
            CustomerId customerId,
            Address shippingAddress,
            Address billingAddress,
            List<(ProductId ProductId, int Quantity, Money UnitPrice)> items)
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
                var orderItem = OrderItem.Create(
                    item.ProductId,
                    item.Quantity,
                    item.UnitPrice);

                order._items.Add(orderItem);
            }

            order.CalculateTotals();
            order.RaiseDomainEvent(new OrderCreatedDomainEvent(orderId, customerId, orderNumber));

            return Result.Success(order);
        }

        public Result AddItem(ProductId productId, int quantity, Money unitPrice)
        {
            if (Status != OrderStatus.Pending)
                return Result.Failure(OrderErrors.CannotModifyConfirmedOrder);

            if (quantity <= 0)
                return Result.Failure(OrderErrors.InvalidQuantity);

            var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var newItem = OrderItem.Create(productId, quantity, unitPrice);
                _items.Add(newItem);
            }

            CalculateTotals();
            return Result.Success();
        }

        public Result RemoveItem(ProductId productId)
        {
            if (Status != OrderStatus.Pending)
                return Result.Failure(OrderErrors.CannotModifyConfirmedOrder);

            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                return Result.Failure(OrderErrors.ItemNotFound);

            _items.Remove(item);
            CalculateTotals();

            return Result.Success();
        }

        public Result Confirm()
        {
            if (Status != OrderStatus.Pending)
                return Result.Failure(OrderErrors.InvalidStatusTransition);

            if (_items.Count == 0)
                return Result.Failure(OrderErrors.EmptyOrder);

            Status = OrderStatus.Confirmed;
            RaiseDomainEvent(new OrderConfirmedDomainEvent(Id, CustomerId, TotalAmount));

            return Result.Success();
        }

        public Result Ship()
        {
            if (Status != OrderStatus.Confirmed)
                return Result.Failure(OrderErrors.InvalidStatusTransition);

            Status = OrderStatus.Shipped;
            RaiseDomainEvent(new OrderShippedDomainEvent(Id, ShippingAddress));

            return Result.Success();
        }

        public Result Deliver()
        {
            if (Status != OrderStatus.Shipped)
                return Result.Failure(OrderErrors.InvalidStatusTransition);

            Status = OrderStatus.Delivered;
            CompletedDate = DateTime.UtcNow;
            RaiseDomainEvent(new OrderDeliveredDomainEvent(Id));

            return Result.Success();
        }

        public Result Cancel(string reason)
        {
            if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
                return Result.Failure(OrderErrors.CannotCancelOrder);

            Status = OrderStatus.Cancelled;
            RaiseDomainEvent(new OrderCancelledDomainEvent(Id, reason));

            return Result.Success();
        }

        private void CalculateTotals()
        {
            SubtotalAmount = _items
                .Select(i => i.TotalPrice)
                .Aggregate(Money.Create(0, "NGN"), (acc, price) => acc.Add(price));

            // Simple tax calculation (10%)
            TaxAmount = Money.Create(SubtotalAmount.Amount * 0.10m, "NGN");

            // Flat shipping rate
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
